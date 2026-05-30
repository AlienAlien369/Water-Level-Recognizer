using Hangfire;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using WLR.Application;
using WLR.Infrastructure;
using WLR.Infrastructure.Services;
using WLR.API.Extensions;
using WLR.API.Middleware;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Fail fast if critical env vars are missing
var jwtKey = builder.Configuration["JwtSettings:SecretKey"];
if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException(
        "JwtSettings:SecretKey is not configured. Set env var JwtSettings__SecretKey.");

var pgCs = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(pgCs))
    throw new InvalidOperationException(
        "ConnectionStrings:DefaultConnection is not configured. Set env var ConnectionStrings__DefaultConnection.");


// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console()
    .WriteTo.File("logs/wlr-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30)
    .CreateLogger();

builder.Host.UseSerilog();

// Services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();

builder.Services.AddSignalR();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:5173" })
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("api", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 10;
    });
    options.AddFixedWindowLimiter("auth", limiterOptions =>
    {
        limiterOptions.PermitLimit = 10;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 2;
    });
});

// Health Checks
var pgCsNorm = WLR.Infrastructure.ConnectionStringHelper.NormalizePostgres(builder.Configuration.GetConnectionString("DefaultConnection"));
var redisCsNorm = WLR.Infrastructure.ConnectionStringHelper.NormalizeRedis(builder.Configuration.GetConnectionString("Redis"));
builder.Services.AddHealthChecks()
    .AddNpgSql(pgCsNorm!)
    .AddRedis(redisCsNorm!);

var app = builder.Build();

// Middleware pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WaterLevelRecognizer API v1");
        c.RoutePrefix = "swagger";
        c.DisplayRequestDuration();
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<MonitoringHub>("/hubs/monitoring").RequireAuthorization();
app.MapHealthChecks("/health");
app.UseHangfireDashboard("/hangfire", new DashboardOptions { IsReadOnlyFunc = _ => false });

// Auto migrate
await app.MigrateAndSeedDatabaseAsync();

app.Run();
