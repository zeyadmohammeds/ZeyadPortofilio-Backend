namespace Portfolio.API.Models;

public sealed record ErrorResponse(bool Success, int StatusCode, string Message, object? Errors = null);
