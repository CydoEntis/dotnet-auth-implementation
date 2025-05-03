using AuthImplementation.Application.Common;
using AuthImplementation.Domain.Entities;
using AuthImplementation.Infrastructure.Common;
using AuthImplementation.Infrastructure.Services.Email.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthImplementation.Application.Features.Auth.ForgotPassword;

public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, Result<ForgotPasswordResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<ForgotPasswordHandler> _logger;
    private readonly IConfiguration _configuration;
    private readonly IValidator<ForgotPasswordCommand> _validator;

    public ForgotPasswordHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        IEmailService emailService,
        ILogger<ForgotPasswordHandler> logger,
        IConfiguration configuration,
        IValidator<ForgotPasswordCommand> validator)
    {
        _userManager = userManager;
        _emailService = emailService;
        _logger = logger;
        _configuration = configuration;
        _validator = validator;
    }

    public async Task<Result<ForgotPasswordResponse>> Handle(ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult =
            await ValidationHelper.ValidateAsync<ForgotPasswordCommand, ForgotPasswordResponse>(_validator, request);
        if (validationResult is not null) return validationResult;

        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                _logger.LogWarning("Password reset attempt failed for email {Email}", request.Email);
                return Result<ForgotPasswordResponse>.Success(new ForgotPasswordResponse()
                    { Message = "An email has been sent to your email address." });
            }

            var appName = _configuration["Application:Name"] ?? "YourApp";
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(resetToken);
            var forgotPasswordUrl = $"https://localhost:5173/reset-password?token={encodedToken}";

            var templateData = new Dictionary<string, string>
            {
                ["APP_NAME"] = appName,
                ["RECIPIENT'S EMAIL"] = request.Email,
                ["RESET_LINK"] = forgotPasswordUrl
            };

            await _emailService.SendEmailFromTemplateAsync(
                toEmail: request.Email,
                subject: "Forgot your password",
                fromName: appName,
                templateFileName: "ForgotPasswordEmailTemplate.html",
                templateData: templateData
            );


            return Result<ForgotPasswordResponse>.Success(new ForgotPasswordResponse()
                { Message = "An email has been sent to your email address." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing a forgot password request for email {Email}",
                request.Email);

            return Result<ForgotPasswordResponse>.Failure(new ApiError()
            {
                Code = ErrorCode.InternalError,
                Message = "Something went wrong while processing your request. Please try again later."
            });
        }
    }
}