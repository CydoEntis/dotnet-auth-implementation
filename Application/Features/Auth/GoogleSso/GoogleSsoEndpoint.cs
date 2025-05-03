using AuthImplementation.Application.Common;
using AuthImplementation.Infrastructure.Common;
using MediatR;

namespace AuthImplementation.Application.Features.Auth.GoogleSso;

public static class GoogleSsoEndpoint
{
    public static void MapGoogleSsoEndpoint(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/api/auth/google-login",
                async (GoogleSsoCommand command, IMediator mediator, HttpResponse response) =>
                {
                    var result = await mediator.Send(command);

                    return ResultMapper.ToHttpResult(result, response,
                        (data, res) =>
                        {
                            CookieHelper.SetRefreshTokenCookie(res, data.RefreshToken, data.RefreshTokenExpiration);
                        });
                })
            .WithName("GoogleLogin")
            .WithTags("Auth")
            .Produces<ApiResponse<GoogleSsoResponse>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<GoogleSsoResponse>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<GoogleSsoResponse>>(StatusCodes.Status404NotFound)
            .Produces<ApiResponse<GoogleSsoResponse>>(StatusCodes.Status409Conflict)
            .Produces<ApiResponse<GoogleSsoResponse>>(StatusCodes.Status500InternalServerError);
    }
}