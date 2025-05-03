using AuthImplementation.Application.Common;
using AuthImplementation.Infrastructure.Common;
using MediatR;

namespace AuthImplementation.Application.Features.Auth.ChangePassword;

public static class ChangePasswordEndpoint
{
    public static void MapChangePasswordEndpoint(this IEndpointRouteBuilder routes)
    {
        routes.MapPut("/api/auth/change-password", async (ChangePasswordCommand command, IMediator mediator) =>
            {
                var result = await mediator.Send(command);
                return ResultMapper.ToHttpResult(result);
            })
            .RequireAuthorization()
            .WithName("ChangePassword")
            .WithTags("Auth")
            .Produces<ApiResponse<ChangePasswordResponse>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<ChangePasswordResponse>>(StatusCodes.Status400BadRequest)
            .Produces<
                ApiResponse<ChangePasswordResponse>>(StatusCodes.Status404NotFound)
            .Produces<
                ApiResponse<ChangePasswordResponse>>(StatusCodes.Status500InternalServerError);
    }
}