using AuthImplementation.Application.Common;
using AuthImplementation.Infrastructure.Common;
using MediatR;

namespace AuthImplementation.Application.Features.Auth.Register;

public static class RegisterEndpoint
{
    public static void MapRegisterEndpoint(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/api/auth/register", async (RegisterCommand command, IMediator mediator) =>
            {
                var result = await mediator.Send(command);
                return ResultMapper.ToHttpResult(result);
            })
            .WithName("Register")
            .WithTags("Auth")
            .Produces<ApiResponse<RegisterResponse>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<RegisterResponse>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<RegisterResponse>>(StatusCodes.Status500InternalServerError);
    }
}