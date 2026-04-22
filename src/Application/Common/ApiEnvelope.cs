namespace Portfolio.Application.Common;

public sealed record ApiEnvelope<T>(bool Success, int StatusCode, string Message, T? Data = default, object? Errors = null);

public sealed record PagedResult<T>(IReadOnlyCollection<T> Items, int TotalCount, int Page, int PageSize);
