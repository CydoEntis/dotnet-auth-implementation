using System.Text.Json.Serialization;

namespace AuthImplementation.Application.Features.Auth.GoogleSso;

public class GoogleSsoResponse
{
    public string AccessToken { get; init; } = string.Empty;
    [JsonIgnore]
    public string RefreshToken { get; init; } = string.Empty;
    [JsonIgnore] 
    public DateTime RefreshTokenExpiration { get; set; }
}