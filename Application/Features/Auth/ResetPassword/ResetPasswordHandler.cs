using AuthImplementation.Application.Common;
using AuthImplementation.Domain.Entities;
using AuthImplementation.Infrastructure.Common;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthImplementation.Application.Features.Auth.ResetPassword;

public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Result<ResetPasswordResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<ResetPasswordHandler> _logger;
    private readonly IValidator<ResetPasswordCommand> _validator;

    public ResetPasswordHandler(
        UserManager<AppUser> userManager,
        ILogger<ResetPasswordHandler> logger,
        IValidator<ResetPasswordCommand> validator)
    {
        _userManager = userManager;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<ResetPasswordResponse>> Handle(ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult =
            await ValidationHelper.ValidateAsync<ResetPasswordCommand, ResetPasswordResponse>(_validator, request);
        if (validationResult is not null) return validationResult;

        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                _logger.LogWarning("Password reset failed: No user found for email {Email}", request.Email);
                var validationErrors = new List<ValidationError>();
                validationErrors.Add(new ValidationError()
                    { Message = "Email not found", Property = "Email" });
                return Result<ResetPasswordResponse>.Failure(new ApiError()
                {
                    Code = ErrorCode.ValidationError,
                    Message = "One or more validation errors occurred",
                    Details = validationErrors
                });
            }

            var decodedToken = Uri.UnescapeDataString(request.Token);
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Password reset failed for user {Email}: {@Errors}", request.Email, result.Errors);

                return Result<ResetPasswordResponse>.Failure(new ApiError()
                {
                    Code = ErrorCode.NotFound,
                    Message = "Failed to reset password. Your link may have expired or is invalid.",
                });
            }

            return Result<ResetPasswordResponse>.Success(new ResetPasswordResponse()
                { Message = "Password has been successfully reset." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while resetting password for email {Email}", request.Email);
            return Result<ResetPasswordResponse>.Failure(new ApiError()
            {
                Code = ErrorCode.InternalError,
                Message = "An error occured while resetting your password"
            });
        }
    }
}