using AuthImplementation.Application.Common;
using AuthImplementation.Infrastructure.Common;
using MediatR;

namespace AuthImplementation.Application.Features.Auth.ResetPassword;

public static class ResetPasswordEndpoint
{
    public static void MapResetPasswordEndpoint(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/api/auth/reset-password", async (ResetPasswordCommand command, IMediator mediator) =>
            {
                var result = await mediator.Send(command);
                return ResultMapper.ToHttpResult(result);
            })
            .WithName("ResetPassword")
            .WithTags("Auth")
            .Produces<ApiResponse<Unit>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<Unit>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<Unit>>(StatusCodes.Status404NotFound)
            .Produces<ApiResponse<Unit>>(StatusCodes.Status500InternalServerError);
    }
}