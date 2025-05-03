namespace AuthImplementation.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsExtension(this IServiceCollection services, IConfiguration config)
    {
        var allowedOrigins = config.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        var exposedHeaders = config.GetSection("Cors:ExposedHeaders").Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials()
                      .WithExposedHeaders(exposedHeaders);
            });
        });

        return services;
    }
}