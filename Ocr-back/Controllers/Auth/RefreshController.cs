using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ocr_back.Controllers.Auth.Dtos;
using Ocr_back.Data;
using Ocr_back.Services;

namespace Ocr_back.Controllers.Auth;

[ApiController]
[Route("[controller]")]
public class RefreshController : Controller
{
    private readonly TokenService _tokenService;
    private readonly AuthDbContext _db;

    public RefreshController(TokenService tokenService, AuthDbContext db)
    {
        _tokenService = tokenService;
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Refresh([FromBody]RefreshTokenRequest refreshToken)
    {
        var token = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken.RefreshToken);
        if (token == null) return Unauthorized();
        
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == token.UserId);
        if (user == null) return Unauthorized();

        if (token.ExpiresAt < DateTime.UtcNow) return Unauthorized();
        
        var newToken = await _tokenService.GenerateJwtToken(user);
        return Ok(new TokenResponse()
        {
            AccessToken = newToken,
            RefreshToken = refreshToken.RefreshToken
        });
    }
    
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; }
}









