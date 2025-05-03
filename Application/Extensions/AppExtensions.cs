using System.Reflection;
using FluentValidation.AspNetCore;

namespace AuthImplementation.Application.Extensions;

public static class AppExtensions
{
    public static IServiceCollection AddAppExtension(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Program>());

        return services;
    }
}