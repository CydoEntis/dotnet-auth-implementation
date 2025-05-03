using System.Text.Json.Serialization;

namespace AuthImplementation.Application.Features.Auth.Login;

public class LoginResponse
{
    public string AccessToken { get; init; } = string.Empty;
    [JsonIgnore]
    public string RefreshToken { get; init; } = string.Empty;
    [JsonIgnore] 
    public DateTime RefreshTokenExpiration { get; set; }
    
}