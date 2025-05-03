using AuthImplementation.Application.Common;
using MediatR;

namespace AuthImplementation.Application.Features.Auth.ResetPassword;

public record ResetPasswordCommand(
    string Email,
    string Token,
    string NewPassword
) : IRequest<Result<ResetPasswordResponse>>;