using AuthImplementation.Application.Common;
using MediatR;

namespace AuthImplementation.Application.Features.Auth.ChangePassword;

public record ChangePasswordCommand(string CurrentPassword, string NewPassword)
    : IRequest<Result<ChangePasswordResponse>>;