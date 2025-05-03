using AuthImplementation.Infrastructure.Common;

namespace AuthImplementation.Application.Common;

public static class ErrorMapper
{
    public static IResult ToApiResult(ApiError error)
    {
        var apiResponse = ApiResponse<object>.FromError(error);

        return error.Code switch
        {
            ErrorCode.ValidationError => Results.BadRequest(apiResponse),
            ErrorCode.Conflict => Results.Conflict(apiResponse),
            ErrorCode.NotFound => Results.NotFound(apiResponse),
            ErrorCode.Unauthorized => Results.Unauthorized(),
            ErrorCode.Forbidden => Results.Json(apiResponse,
                statusCode: StatusCodes.Status403Forbidden),
            ErrorCode.ServiceUnavailable => Results.Json(apiResponse,
                statusCode: StatusCodes.Status503ServiceUnavailable),
            _ => Results.Json(apiResponse, statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}