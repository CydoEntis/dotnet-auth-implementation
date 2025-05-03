using AuthImplementation.Infrastructure.Common;

namespace AuthImplementation.Application.Common;

public static class ResultMapper
{
    public static IResult ToHttpResult<T>(Result<T> result)
    {
        if (!result.IsSuccess)
            return ErrorMapper.ToApiResult(result.Error);

        return Results.Ok(ApiResponse<T>.FromSuccess(result.Data));
    }

    public static IResult ToHttpResult<T>(
        Result<T> result,
        HttpResponse response,
        Action<T, HttpResponse> onSuccess)
    {
        if (!result.IsSuccess)
            return ErrorMapper.ToApiResult(result.Error);

        onSuccess(result.Data, response);

        return Results.Ok(ApiResponse<T>.FromSuccess(result.Data));
    }
}