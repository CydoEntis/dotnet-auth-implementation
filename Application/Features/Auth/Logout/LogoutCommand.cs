using AuthImplementation.Application.Common;
using MediatR;

namespace AuthImplementation.Application.Features.Auth.Logout;

public record LogoutCommand : IRequest<Result<LogoutResponse>>;