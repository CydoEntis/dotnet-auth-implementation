using AuthImplementation.Application.Common;
using AuthImplementation.Infrastructure.Common;
using MediatR;

namespace AuthImplementation.Application.Features.Auth.Login;

public static class LoginEndpoint
{
    public static void MapLoginEndpoint(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/api/auth/login", async (LoginCommand command, IMediator mediator, HttpResponse response) =>
            {
                var result = await mediator.Send(command);

                return ResultMapper.ToHttpResult(result, response,
                    (data, res) =>
                    {
                        CookieHelper.SetRefreshTokenCookie(res, data.RefreshToken, data.RefreshTokenExpiration);
                    });
            })
            .WithName("Login")
            .WithTags("Auth")
            .Produces<ApiResponse<LoginResponse>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<LoginResponse>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<LoginResponse>>(StatusCodes.Status401Unauthorized);
    }
}