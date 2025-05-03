namespace AuthImplementation.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public string Token { get; set; }
    public DateTime ExpirationDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}