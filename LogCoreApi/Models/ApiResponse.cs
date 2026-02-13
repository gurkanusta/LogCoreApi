namespace LogCoreApi.Models;

public class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public ApiError? Error { get; init; }
    public string? TraceId { get; init; }
    public string? CorrelationId { get; init; }

    public static ApiResponse<T> Ok(T data, HttpContext ctx) => new()
    {
        Success = true,
        Data = data,
        TraceId = ctx.TraceIdentifier,
        CorrelationId = ctx.Items.TryGetValue("CorrelationId", out var cid) ? cid?.ToString() : null
    };

    public static ApiResponse<T> Fail(string code, string message, HttpContext ctx, object? details = null) => new()
    {
        Success = false,
        Error = new ApiError { Code = code, Message = message, Details = details },
        TraceId = ctx.TraceIdentifier,
        CorrelationId = ctx.Items.TryGetValue("CorrelationId", out var cid) ? cid?.ToString() : null
    };
}

public class ApiError
{
    public string Code { get; init; } = default!;
    public string Message { get; init; } = default!;
    public object? Details { get; init; }
}