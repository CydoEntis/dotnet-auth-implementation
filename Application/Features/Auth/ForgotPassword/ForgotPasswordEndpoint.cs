using AuthImplementation.Application.Common;
using AuthImplementation.Infrastructure.Common;
using MediatR;

namespace AuthImplementation.Application.Features.Auth.ForgotPassword;

public static class ForgotPasswordEndpoint
{
    public static void MapForgotPasswordEndpoint(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/api/auth/forgot-password", async (ForgotPasswordCommand command, IMediator mediator) =>
            {
                var result = await mediator.Send(command);
                return ResultMapper.ToHttpResult(result);
            })
            .WithName("ForgotPassword")
            .WithTags("Auth")
            .Produces<ApiResponse<ForgotPasswordResponse>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<ForgotPasswordResponse>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<ForgotPasswordResponse>>(StatusCodes.Status500InternalServerError);
    }
}