using AuthImplementation.Domain.Entities;

namespace AuthImplementation.Infrastructure.Services.Identity.Interfaces;

public interface ITokenService
{
    Task<RefreshToken?> GetRefreshTokenAsync(string token);
    Task SaveRefreshTokenAsync(string token, DateTime expirationDate, string userId);
    string CreateAccessToken(AppUser user, DateTime expirationTime);

    string CreateRefreshToken();
    Task InvalidateRefreshTokenAsync(string token);
}