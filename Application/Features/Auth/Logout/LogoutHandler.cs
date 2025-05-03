using AuthImplementation.Application.Common;
using AuthImplementation.Infrastructure.Common;
using AuthImplementation.Infrastructure.Services.Identity.Interfaces;
using MediatR;

namespace AuthImplementation.Application.Features.Auth.Logout
{
    public class LogoutHandler : IRequestHandler<LogoutCommand, Result<LogoutResponse>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenService _tokenService;
        private readonly ILogger<LogoutHandler> _logger;

        public LogoutHandler(IHttpContextAccessor httpContextAccessor, ITokenService tokenService,
            ILogger<LogoutHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<Result<LogoutResponse>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    _logger.LogWarning("Logout attempt failed: User is not logged in.");
                    return Result<LogoutResponse>.Failure(new ApiError()
                    {
                        Code = ErrorCode.NotFound,
                        Message = "User is not logged in"
                    });
                }

                var userId = httpContext.User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Logout attempt failed: User is not logged in.");
                    return Result<LogoutResponse>.Failure(new ApiError()
                    {
                        Code = ErrorCode.NotFound,
                        Message = "User is not logged in"
                    });
                }

                var refreshToken = httpContext.Request.Cookies["refresh_token"];

                if (string.IsNullOrEmpty(refreshToken))
                {
                    return Result<LogoutResponse>.Success(new LogoutResponse()
                        { Message = "User logged out successfully." });
                }

                var retrievedToken = await _tokenService.GetRefreshTokenAsync(refreshToken);

                if (retrievedToken == null || retrievedToken.Token != refreshToken)
                {
                    _logger.LogInformation("Logout successful: No refresh token found, user already logged out.");
                    return Result<LogoutResponse>.Success(new LogoutResponse()
                        { Message = "User logged out successfully." });
                }

                await _tokenService.InvalidateRefreshTokenAsync(refreshToken);

                _logger.LogInformation("Logout successful: Refresh token invalidated.");
                return Result<LogoutResponse>.Success(new LogoutResponse()
                    { Message = "User logged out successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred during logout.");
                return Result<LogoutResponse>.Failure(new ApiError()
                {
                    Code = ErrorCode.InternalError,
                    Message = "An error occured during logout."
                });
            }
        }
    }
}