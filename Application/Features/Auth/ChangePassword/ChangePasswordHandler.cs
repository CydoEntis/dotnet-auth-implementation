using AuthImplementation.Application.Common;
using AuthImplementation.Domain.Entities;
using AuthImplementation.Infrastructure.Common;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthImplementation.Application.Features.Auth.ChangePassword;

public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, Result<ChangePasswordResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<ChangePasswordHandler> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IValidator<ChangePasswordCommand> _validator;

    public ChangePasswordHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        ILogger<ChangePasswordHandler> logger,
        IValidator<ChangePasswordCommand> validator)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<ChangePasswordResponse>> Handle(ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult =
            await ValidationHelper.ValidateAsync<ChangePasswordCommand, ChangePasswordResponse>(_validator, request);
        if (validationResult is not null) return validationResult;

        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                _logger.LogWarning("Logout attempt failed: User is not logged in.");
                return Result<ChangePasswordResponse>.Failure(new ApiError()
                {
                    Code = ErrorCode.NotFound,
                    Message = "User is not logged in"
                });
            }

            var userId = httpContext.User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Logout attempt failed: User is not logged in.");
                return Result<ChangePasswordResponse>.Failure(new ApiError()
                {
                    Code = ErrorCode.NotFound,
                    Message = "User is not logged in"
                });
            }


            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                _logger.LogWarning("Password reset attempt failed for email {Email}", user.Email);
                return Result<ChangePasswordResponse>.Failure(new ApiError()
                {
                    Code = ErrorCode.NotFound,
                    Message = "User could not be found"
                });
            }

            if (!await _userManager.CheckPasswordAsync(user, request.CurrentPassword))
            {
                _logger.LogWarning("Password check failed: Passwords do not match.");
                var validationErrors = new List<ValidationError>();
                validationErrors.Add(new ValidationError()
                    { Message = "Passwords do not match", Property = "currentPassword" });
                return Result<ChangePasswordResponse>.Failure(new ApiError()
                {
                    Code = ErrorCode.ValidationError,
                    Message = "One or more validation errors occurred",
                    Details = validationErrors
                });
            }

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Password change failed");
                return Result<ChangePasswordResponse>.Failure(new ApiError()
                {
                    Code = ErrorCode.InternalError,
                    Message = "Failed to change password"
                });
            }

            return Result<ChangePasswordResponse>.Success(new ChangePasswordResponse()
                { Message = "Your password has been successfully changed." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while changing password");
            return Result<ChangePasswordResponse>.Failure(new ApiError()
            {
                Code = ErrorCode.InternalError,
                Message = "An error occured while changing your password"
            });
        }
    }
}