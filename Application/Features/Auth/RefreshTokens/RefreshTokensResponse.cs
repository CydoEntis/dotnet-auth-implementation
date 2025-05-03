using System.Text.Json.Serialization;

namespace AuthImplementation.Application.Features.Auth.RefreshTokens;

public class RefreshTokensResponse
{
    public string AccessToken { get; set; } = string.Empty;
    [JsonIgnore]
    public string RefreshToken { get; init; } = string.Empty;
    [JsonIgnore] 
    public DateTime RefreshTokenExpiration { get; set; }
}