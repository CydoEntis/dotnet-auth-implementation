using AuthImplementation.Application.Common;
using MediatR;

namespace AuthImplementation.Application.Features.Auth.Register;

public record RegisterCommand(string Email, string FirstName, string LastName, string Password)
    : IRequest<Result<RegisterResponse>>;