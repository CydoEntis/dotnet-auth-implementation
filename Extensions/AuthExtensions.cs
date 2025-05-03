using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using AuthImplementation.Domain.Entities;
using AuthImplementation.Infrastructure.Persistence;


using Microsoft.AspNetCore.Authentication.Google;


namespace AuthImplementation.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddAuthExtension(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddIdentityCore<AppUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
            
        var authBuilder = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = configuration.GetValue<bool>("Jwt:ValidateIssuer"),
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidateAudience = configuration.GetValue<bool>("Jwt:ValidateAudience"),
                ValidAudience = configuration["Jwt:Audience"],
                ValidateLifetime = configuration.GetValue<bool>("Jwt:ValidateLifetime"),
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!))
            };
        });

        
        authBuilder.AddGoogle(options =>
        {
            options.ClientId = configuration["Auth:Google:ClientId"];
            options.ClientSecret = configuration["Auth:Google:ClientSecret"];
            options.CallbackPath = configuration["Auth:Google:CallbackPath"];
        });
        

        return services;
    }
}
