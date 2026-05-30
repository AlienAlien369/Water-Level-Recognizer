using System.Net;
using System.Text.Json;
using FluentValidation;
using WLR.Application.Common.Models;
using WLR.Domain.Exceptions;

namespace WLR.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Business-rule exceptions are expected — log at Warning, not Error
            if (ex is DomainException or NotFoundException or ForbiddenException or ConflictException
                || ex is ValidationException or UnauthorizedAccessException)
                _logger.LogWarning("Handled exception ({Type}): {Message}", ex.GetType().Name, ex.Message);
            else
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var (statusCode, message, errors) = exception switch
        {
            ValidationException ve => (HttpStatusCode.BadRequest, "Validation failed", ve.Errors.Select(e => e.ErrorMessage)),
            NotFoundException nfe => (HttpStatusCode.NotFound, nfe.Message, (IEnumerable<string>?)null),
            ForbiddenException fe => (HttpStatusCode.Forbidden, fe.Message, (IEnumerable<string>?)null),
            ConflictException ce => (HttpStatusCode.Conflict, ce.Message, (IEnumerable<string>?)null),
            DomainException de => (HttpStatusCode.BadRequest, de.Message, (IEnumerable<string>?)null),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized", (IEnumerable<string>?)null),
            _ => (HttpStatusCode.InternalServerError, "An internal server error occurred.", (IEnumerable<string>?)null)
        };

        context.Response.StatusCode = (int)statusCode;
        var response = ApiResponse.Fail(message, errors);
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}
