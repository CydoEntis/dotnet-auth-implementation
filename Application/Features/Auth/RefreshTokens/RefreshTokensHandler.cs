using AuthImplementation.Application.Common;
using AuthImplementation.Domain.Entities;
using AuthImplementation.Infrastructure.Common;
using AuthImplementation.Infrastructure.Services.Identity.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthImplementation.Application.Features.Auth.RefreshTokens
{
    public class RefreshTokensHandler
        : IRequestHandler<RefreshTokensCommand, Result<RefreshTokensResponse>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RefreshTokensHandler> _logger;

        public RefreshTokensHandler(IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager, ITokenService tokenService, IConfiguration configuration,
            ILogger<RefreshTokensHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _tokenService = tokenService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<Result<RefreshTokensResponse>> Handle(RefreshTokensCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    _logger.LogWarning("Logout attempt failed: User is not logged in.");
                    return Result<RefreshTokensResponse>.Failure(new ApiError()
                    {
                        Code = ErrorCode.NotFound,
                        Message = "User is not logged in"
                    });
                }

                var userId = httpContext.User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Logout attempt failed: User is not logged in.");
                    return Result<RefreshTokensResponse>.Failure(new ApiError()
                    {
                        Code = ErrorCode.NotFound,
                        Message = "User is not logged in"
                    });
                }

                var refreshToken = httpContext.Request.Cookies["refresh_token"];
                if (string.IsNullOrEmpty(refreshToken))
                {
                    _logger.LogWarning("Refresh token request failed: No matching refresh token found.");
                    return Result<RefreshTokensResponse>.Failure(new ApiError()
                    {
                        Code = ErrorCode.NotFound,
                        Message = "No matching refresh token found."
                    });
                }

                var token = await _tokenService.GetRefreshTokenAsync(refreshToken);
                if (token == null)
                {
                    _logger.LogWarning("Refresh token request failed: Token not found.");
                    return Result<RefreshTokensResponse>.Failure(new ApiError()
                    {
                        Code = ErrorCode.NotFound,
                        Message = "No matching refresh token found."
                    });
                }

                var user = await _userManager.FindByIdAsync(token.UserId);
                if (user == null)
                {
                    _logger.LogWarning("Refresh token request failed: User not found.");
                    return Result<RefreshTokensResponse>.Failure(new ApiError()
                    {
                        Code = ErrorCode.NotFound,
                        Message = "User could not be found."
                    });
                }

                await _tokenService.InvalidateRefreshTokenAsync(refreshToken);

                var newAccessTokenExpiration =
                    DateTime.UtcNow.AddHours(_configuration.GetValue<double>("Jwt:AccessTokenExpirationInHours"));
                var newAccessToken = _tokenService.CreateAccessToken(user, newAccessTokenExpiration);

                var newRefreshTokenExpiration =
                    DateTime.UtcNow.AddHours(_configuration.GetValue<double>("Jwt:RefreshTokenExpirationInHours"));
                var newRefreshToken = _tokenService.CreateRefreshToken();

                await _tokenService.SaveRefreshTokenAsync(refreshToken, newRefreshTokenExpiration, user.Id);

                var response = new RefreshTokensResponse
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    RefreshTokenExpiration = newRefreshTokenExpiration
                };

                _logger.LogInformation("Refresh token request successful.");
                return Result<RefreshTokensResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred during refresh token request.");
                return Result<RefreshTokensResponse>.Failure(new ApiError()
                {
                    Code = ErrorCode.InternalError,
                    Message = "An error occured while changing your password"
                });
            }
        }
    }
}