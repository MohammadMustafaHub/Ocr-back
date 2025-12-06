using System.Security.Cryptography;

namespace Ocr_back.Models;

public class AccessToken
{
    public Guid Id { get; set; }
    public string Token { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public DateTime? ExpiresAt { get; set; } = null;
    public TokenType TokenType { get; set; }

    public string? TokenName { get; set; }
    

    public static AccessToken Create(User user, TokenType tokenType, DateTime? expiresAt, string? tokenName = null)
    {
        return new AccessToken
        {
            Token = GenerateToken(),
            UserId = user.Id,
            Id = Guid.NewGuid(),
            ExpiresAt = expiresAt,
            TokenType = tokenType,
            TokenName = tokenName
        };
    }
    
    // if null return true
    public bool IsExpired()
    {
        if(ExpiresAt == null) return false;
        
        return DateTime.UtcNow > ExpiresAt;
    }
    
    private static string GenerateToken()
    {
        var rng = RandomNumberGenerator.Create();
        var tokenData = new byte[64];
        rng.GetBytes(tokenData);
        return Convert.ToBase64String(tokenData);
    }
}



