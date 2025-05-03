using AuthImplementation.Application.Features.Auth.ForgotPassword;
using AuthImplementation.Application.Features.Auth.Login;
using AuthImplementation.Application.Features.Auth.Register;
using AuthImplementation.Application.Features.Auth.ResetPassword;
using AuthImplementation.Application.Features.Auth.GoogleSso;
using FluentValidation;

namespace AuthImplementation.Application.Extensions;

public static class ValidationExtensions
{
    public static IServiceCollection AddValidationExtension(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<RegisterCommandValidator>();
        services.AddValidatorsFromAssemblyContaining<LoginCommandValidator>();
        services.AddValidatorsFromAssemblyContaining<ForgotPasswordCommandValidator>();
        services.AddValidatorsFromAssemblyContaining<ResetPasswordCommandValidator>();
        services.AddValidatorsFromAssemblyContaining<GoogleSsoCommandValidator>();


        return services;
    }
}