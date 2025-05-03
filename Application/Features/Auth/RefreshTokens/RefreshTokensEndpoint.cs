using AuthImplementation.Application.Common;
using AuthImplementation.Infrastructure.Common;
using MediatR;

namespace AuthImplementation.Application.Features.Auth.RefreshTokens;

public static class RefreshTokensEndpoint
{
    public static void MapRefreshTokensEndpoint(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/api/auth/refresh-tokens", async (IMediator mediator, HttpResponse response) =>
            {
                var result = await mediator.Send(new RefreshTokensCommand());

                return ResultMapper.ToHttpResult(result, response,
                    (data, res) =>
                    {
                        CookieHelper.SetRefreshTokenCookie(res, data.RefreshToken, data.RefreshTokenExpiration);
                    });
            })
            .RequireAuthorization()
            .WithName("RefreshTokens")
            .WithTags("Auth")
            .Produces<ApiResponse<RefreshTokensResponse>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<RefreshTokensResponse>>(StatusCodes.Status404NotFound)
            .Produces<ApiResponse<RefreshTokensResponse>>(StatusCodes.Status500InternalServerError);
    }
}