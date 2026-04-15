namespace CMS.Shared.Responses;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public IEnumerable<string>? Errors { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "Success") =>
        new() { Success = true, Data = data, Message = message, Errors = null };

    public static ApiResponse<T> Fail(string message, IEnumerable<string>? errors = null) =>
        new() { Success = false, Data = default, Message = message, Errors = errors ?? new[] { message } };

    public static ApiResponse<T> Fail(IEnumerable<string> errors, string message = "One or more errors occurred.") =>
        new() { Success = false, Data = default, Message = message, Errors = errors };
}
