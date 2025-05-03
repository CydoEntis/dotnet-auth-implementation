using AuthImplementation.Application.Common;
using AuthImplementation.Domain.Entities;
using AuthImplementation.Infrastructure.Common;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthImplementation.Application.Features.Auth.Register
{
    public class RegisterHandler : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<RegisterHandler> _logger;
        private readonly IValidator<RegisterCommand> _validator;

        public RegisterHandler(IValidator<RegisterCommand> validator, UserManager<AppUser> userManager,
            ILogger<RegisterHandler> logger)

        {
            _validator = validator;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<Result<RegisterResponse>> Handle(RegisterCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var validationResult =
                    await ValidationHelper.ValidateAsync<RegisterCommand, RegisterResponse>(_validator,
                        request);
                if (validationResult is not null) return validationResult;

                var user = new AppUser
                {
                    Email = request.Email,
                    UserName = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("User registration failed: {Errors}", string.Join(", ", result.Errors));
                    return Result<RegisterResponse>.Failure(new ApiError()
                    {
                        Code = ErrorCode.InternalError,
                        Message = "Could not register user."
                    });
                }

                _logger.LogInformation("User registered successfully: {Email}", request.Email);

                return Result<RegisterResponse>.Success(new RegisterResponse()
                    { Message = "Registered Successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred during user registration.");
                return Result<RegisterResponse>.Failure(new ApiError()
                {
                    Code = ErrorCode.InternalError,
                    Message = "An error occured while registering user."
                });
            }
        }
    }
}