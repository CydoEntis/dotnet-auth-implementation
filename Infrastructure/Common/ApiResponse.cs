using AuthImplementation.Application.Common;

namespace AuthImplementation.Infrastructure.Common;

public class ApiError
{
    public ErrorCode Code { get; set; }
    public string Message { get; set; }
    public List<ValidationError>? Details { get; set; }
}

public class ValidationError
{
    public string Property { get; set; }
    public string Message { get; set; }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public ApiError? Error { get; set; }

    public static ApiResponse<T> FromSuccess(T data) => new ApiResponse<T> { Success = true, Data = data };

    public static ApiResponse<T> FromError(ApiError error) =>
        new ApiResponse<T> { Success = false, Error = error };
}