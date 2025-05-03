using AuthImplementation.Infrastructure.Common;

namespace AuthImplementation.Application.Common;

public class Result<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public ApiError? Error { get; init; }

    public static Result<T> Success(T data) => new() { IsSuccess = true, Data = data };

    public static Result<T> Failure(ApiError error) => new() { IsSuccess = false, Error = error };
}

public class Result
{
    public bool IsSuccess { get; init; }
    public ApiError? Error { get; init; }

    public static Result Success() => new() { IsSuccess = true };

    public static Result Failure(ApiError error) => new() { IsSuccess = false, Error = error };
}