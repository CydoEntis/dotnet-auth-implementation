using AuthImplementation.Application.Common;
using AuthImplementation.Infrastructure.Common;
using MediatR;

namespace AuthImplementation.Application.Features.Auth.Logout;

public static class LogoutEndpoint
{
    public static void MapLogoutEndpoint(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/api/auth/logout", async (IMediator mediator, HttpResponse response) =>
            {
                var result = await mediator.Send(new LogoutCommand());
                return ResultMapper.ToHttpResult(result, response,
                    (data, res) => { res.Cookies.Delete("refresh_token"); });
            })
            .RequireAuthorization()
            .WithName("Logout")
            .WithTags("Auth")
            .Produces<ApiResponse<LogoutResponse>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<LogoutResponse>>(StatusCodes.Status404NotFound)
            .Produces<ApiResponse<LogoutResponse>>(StatusCodes.Status500InternalServerError);
    }
}