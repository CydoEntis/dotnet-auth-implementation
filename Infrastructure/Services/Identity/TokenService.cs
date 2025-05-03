using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthImplementation.Domain.Entities;
using AuthImplementation.Infrastructure.Persistence;
using AuthImplementation.Infrastructure.Services.Identity.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AuthImplementation.Infrastructure.Services.Identity;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;

    public TokenService(IConfiguration configuration, AppDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);
    }

    public async Task SaveRefreshTokenAsync(string token, DateTime expirationDate, string userId)
    {
        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = token,
            ExpirationDate = expirationDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _context.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
    }

    public string CreateAccessToken(AppUser user, DateTime expirationTime)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("userId", user.Id)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expirationTime,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string CreateRefreshToken()
    {
        return Guid.NewGuid().ToString();
    }

    public async Task InvalidateRefreshTokenAsync(string token)
    {
        var refreshToken = await _context.RefreshTokens
            .Where(r => r.Token == token)
            .FirstOrDefaultAsync();

        if (refreshToken != null)
        {
            refreshToken.ExpirationDate = DateTime.UtcNow;
            refreshToken.UpdatedAt = DateTime.UtcNow;

            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }
    }
}