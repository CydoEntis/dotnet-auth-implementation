using AuthImplementation.Application.Common;
using AuthImplementation.Domain.Entities;
using AuthImplementation.Infrastructure.Common;
using AuthImplementation.Infrastructure.Services.Identity.Interfaces;
using FluentValidation;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthImplementation.Application.Features.Auth.GoogleSso;

public class GoogleSsoHandler : IRequestHandler<GoogleSsoCommand, Result<GoogleSsoResponse>>
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly ILogger<GoogleSsoHandler> _logger;
    private readonly IValidator<GoogleSsoCommand> _validator;

    public GoogleSsoHandler(
        IConfiguration configuration,
        UserManager<AppUser> userManager,
        ITokenService tokenService,
        IValidator<GoogleSsoCommand> validator,
        ILogger<GoogleSsoHandler> logger)
    {
        _configuration = configuration;
        _userManager = userManager;
        _tokenService = tokenService;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<GoogleSsoResponse>> Handle(GoogleSsoCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var validationResult =
                await ValidationHelper.ValidateAsync<GoogleSsoCommand, GoogleSsoResponse>(_validator, request);
            if (validationResult is not null) return validationResult;

            var googleUser = await ExtractUserInfoFromAuthCodeAsync(request.AuthorizationCode);
            if (googleUser is null)
            {
                _logger.LogWarning("Invalid Google token provided.");
                return Result<GoogleSsoResponse>.Failure(new ApiError()
                {
                    Code = ErrorCode.NotFound,
                    Message = "Token is either missing, invalid or expired"
                });
            }

            var user = await _userManager.FindByEmailAsync(googleUser.Email);
            if (user is not null)
            {
                _logger.LogWarning("Registration attempt failed: user with email {Email} already exists.",
                    googleUser.Email);

                return Result<GoogleSsoResponse>.Failure(new ApiError()
                {
                    Code = ErrorCode.Conflict,
                    Message = "An account with that email address already exists"
                });
            }

            var newUser = new AppUser
            {
                UserName = googleUser.Email,
                Email = googleUser.Email,
                FirstName = googleUser.FirstName,
                LastName = googleUser.LastName
            };

            var createResult = await _userManager.CreateAsync(newUser);
            if (!createResult.Succeeded)
            {
                _logger.LogWarning("Failed to create user {Email}: {Error}", googleUser.Email,
                    createResult.Errors.FirstOrDefault()?.Description);

                return Result<GoogleSsoResponse>.Failure(new ApiError()
                {
                    Code = ErrorCode.InternalError,
                    Message = "An error occurred when creating user"
                });
            }

            var accessTokenExpiration =
                DateTime.UtcNow.AddHours(_configuration.GetValue<double>("Jwt:AccessTokenExpirationInHours"));
            var accessToken = _tokenService.CreateAccessToken(newUser, accessTokenExpiration);

            var refreshTokenExpiration =
                DateTime.UtcNow.AddHours(_configuration.GetValue<double>("Jwt:RefreshTokenExpirationInHours"));
            var refreshToken = _tokenService.CreateRefreshToken();

            await _tokenService.SaveRefreshTokenAsync(refreshToken, refreshTokenExpiration, newUser.Id);

            var response = new GoogleSsoResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = refreshTokenExpiration
            };

            _logger.LogInformation("Google SSO user {Email} created and logged in successfully.", newUser.Email);
            return Result<GoogleSsoResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during Google SSO login.");

            return Result<GoogleSsoResponse>.Failure(new ApiError()
            {
                Code = ErrorCode.InternalError,
                Message = "An unexpected error occurred. Please try again later."
            });
        }
    }

    private async Task<GoogleUser?> ExtractUserInfoFromAuthCodeAsync(string authCode)
    {
        try
        {
            var clientId = _configuration["Auth:Google:ClientId"];
            var clientSecret = _configuration["Auth:Google:ClientSecret"];

            var scopes = new[] { "openid", "profile", "email" };

            // This must be changed once you hook up your api to a client, the uri should be where your client redirects to.
            // For example: https://localhost:5173/google-login
            var redirectUri = _configuration["Auth:Google:RedirectUri"];

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                },
                Scopes = scopes
            });

            var tokenResponse = await flow.ExchangeCodeForTokenAsync(
                "user",
                authCode,
                redirectUri,
                CancellationToken.None);

            var payload = await GoogleJsonWebSignature.ValidateAsync(tokenResponse.IdToken);

            return new GoogleUser()
            {
                Email = payload.Email,
                FirstName = payload.GivenName,
                LastName = payload.FamilyName,
                GoogleId = payload.Subject
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exchanging token: {ex.Message}");
            return null;
        }
    }
}