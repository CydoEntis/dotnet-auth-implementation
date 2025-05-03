namespace AuthImplementation.Application.Common
{
    public static class CookieHelper
    {
        public static void SetRefreshTokenCookie(HttpResponse response, string refreshToken, DateTime expirationTime)
        {
            response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = expirationTime,
                Path = "/"
            });
        }
    }
}