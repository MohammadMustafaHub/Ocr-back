using System.Security.Cryptography;

namespace Ocr_back.Models;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public DateTime ExpiresAt { get; set; }

    public static RefreshToken Create(User user)
    {
        return new RefreshToken
        {
            Token = GenerateToken(),
            UserId = user.Id,
            Id = Guid.NewGuid(),
            ExpiresAt = DateTime.UtcNow.AddDays(7).ToUniversalTime()
        };
    }
    
    
    private static string GenerateToken()
    {
        var rng = RandomNumberGenerator.Create();
        var tokenData = new byte[64];
        rng.GetBytes(tokenData);
        return Convert.ToBase64String(tokenData);
    }
}