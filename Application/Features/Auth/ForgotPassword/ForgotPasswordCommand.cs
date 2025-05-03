using AuthImplementation.Application.Common;
using MediatR;

namespace AuthImplementation.Application.Features.Auth.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<Result<ForgotPasswordResponse>>;