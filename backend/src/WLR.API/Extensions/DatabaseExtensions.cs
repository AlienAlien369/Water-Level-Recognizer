using Microsoft.EntityFrameworkCore;
using WLR.Domain.Entities;
using WLR.Domain.Enums;
using WLR.Infrastructure.Persistence;
using WLR.Infrastructure.Services;

namespace WLR.API.Extensions;

public static class DatabaseExtensions
{
    public static async Task MigrateAndSeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Database migration completed.");
            await SeedDataAsync(dbContext, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database migration.");
            throw;
        }
    }

    private static async Task SeedDataAsync(ApplicationDbContext context, ILogger logger)
    {
        var pwService = new PasswordService();

        // Backfill password for existing super admin that has no password hash (pre-password-auth seed)
        var existingSuperAdmin = await context.Users
            .FirstOrDefaultAsync(u => u.Role == UserRole.SuperAdmin);

        if (existingSuperAdmin != null)
        {
            if (string.IsNullOrEmpty(existingSuperAdmin.PasswordHash))
            {
                existingSuperAdmin.SetPassword(pwService.Hash("Admin@1234"));
                if (!existingSuperAdmin.IsActive) existingSuperAdmin.Activate();
                await context.SaveChangesAsync();
                logger.LogWarning("Backfilled password for existing Super Admin (+919999999999 / Admin@1234).");
            }
            return;
        }

        logger.LogWarning("Seeding initial data...");

        var superAdmin = User.Create("Super Admin", "+919999999999", "admin@wlr.com", UserRole.SuperAdmin);
        superAdmin.SetPassword(pwService.Hash("Admin@1234"));
        superAdmin.Activate();
        context.Users.Add(superAdmin);

        var center = Center.Create("Main Center", "Primary operational center", "123 Main Street", "New Delhi", "Delhi", "India", "+911234567890", "center@wlr.com");
        context.Centers.Add(center);

        await context.SaveChangesAsync();
        logger.LogWarning("Seeding done. Super Admin: +919999999999 / Admin@1234 — CHANGE PASSWORD IN PRODUCTION.");
    }
}
