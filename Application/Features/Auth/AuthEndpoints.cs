using AuthImplementation.Application.Features.Auth.ChangePassword;
using AuthImplementation.Application.Features.Auth.ForgotPassword;
using AuthImplementation.Application.Features.Auth.Login;
using AuthImplementation.Application.Features.Auth.Logout;
using AuthImplementation.Application.Features.Auth.RefreshTokens;
using AuthImplementation.Application.Features.Auth.Register;
using AuthImplementation.Application.Features.Auth.ResetPassword;



using AuthImplementation.Application.Features.Auth.GoogleSso;


namespace AuthImplementation.Application.Features.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapRegisterEndpoint();
        routes.MapLoginEndpoint();
        routes.MapLogoutEndpoint();
        routes.MapRefreshTokensEndpoint();
        routes.MapForgotPasswordEndpoint();
        routes.MapChangePasswordEndpoint();
        routes.MapResetPasswordEndpoint();
        
        routes.MapGoogleSsoEndpoint();
        
    }
}