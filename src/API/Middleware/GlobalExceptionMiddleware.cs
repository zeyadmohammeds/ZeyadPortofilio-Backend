using System.Text.Json;
using FluentValidation;
using Portfolio.API.Models;

namespace Portfolio.API.Middleware;

public sealed class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning(ex, "Validation failure");
            await WriteError(context, StatusCodes.Status400BadRequest, "Validation failed", ex.Errors.Select(x => new { x.PropertyName, x.ErrorMessage }));
        }
        catch (KeyNotFoundException ex)
        {
            await WriteError(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteError(context, StatusCodes.Status401Unauthorized, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await WriteError(context, StatusCodes.Status500InternalServerError, "An unexpected server error occurred.");
        }
    }

    private static async Task WriteError(HttpContext context, int statusCode, string message, object? errors = null)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        var payload = new ErrorResponse(false, statusCode, message, errors);
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
