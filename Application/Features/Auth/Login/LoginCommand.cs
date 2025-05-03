using AuthImplementation.Application.Common;
using MediatR;

namespace AuthImplementation.Application.Features.Auth.Login;

public record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;