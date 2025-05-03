using AuthImplementation.Application.Common;
using AuthImplementation.Application.Features.Auth.Login;
using AuthImplementation.Domain.Entities;
using AuthImplementation.Infrastructure.Common;
using AuthImplementation.Infrastructure.Services.Identity.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

public class LoginHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<AppUser> _userManager;
    private readonly IValidator<LoginCommand> _validator;
    private readonly ITokenService _tokenService;

    public LoginHandler(IValidator<LoginCommand> validator, UserManager<AppUser> userManager,
        IConfiguration configuration, ITokenService tokenService)
    {
        _validator = validator;
        _userManager = userManager;
        _configuration = configuration;
        _tokenService = tokenService;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await ValidationHelper.ValidateAsync<LoginCommand, LoginResponse>(_validator, request);
        if (validationResult is not null) return validationResult;

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return Result<LoginResponse>.Failure(InvalidCredentialsError);
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
        {
            return Result<LoginResponse>.Failure(InvalidCredentialsError);
        }

        var accessTokenExpiration =
            DateTime.UtcNow.AddHours(_configuration.GetValue<double>("Jwt:AccessTokenExpirationInHours"));
        var accessToken = _tokenService.CreateAccessToken(user, accessTokenExpiration);

        var refreshTokenExpiration =
            DateTime.UtcNow.AddHours(_configuration.GetValue<double>("Jwt:RefreshTokenExpirationInHours"));
        var refreshToken = _tokenService.CreateRefreshToken();

        await _tokenService.SaveRefreshTokenAsync(refreshToken, refreshTokenExpiration, user.Id);

        var response = new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            RefreshTokenExpiration = refreshTokenExpiration
        };

        return Result<LoginResponse>.Success(response);
    }

    private static ApiError InvalidCredentialsError => new()
    {
        Code = ErrorCode.ValidationError,
        Message = "One or more validation errors occurred.",
        Details = new List<ValidationError>
        {
            new() { Property = "Email", Message = "Invalid credentials." }
        }
    };
}