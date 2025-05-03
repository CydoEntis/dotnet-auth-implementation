using Microsoft.EntityFrameworkCore;
using AuthImplementation.Infrastructure.Persistence;

namespace AuthImplementation.Infrastructure.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabaseExtension(this IServiceCollection services, IConfiguration config)
    {
        var builder = new { Configuration = config, Services = services };

        builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(config.GetConnectionString("DefaultConnection")));

        return services;
    }
}
