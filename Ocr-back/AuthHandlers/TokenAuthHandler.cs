using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Ocr_back.Data;
using Ocr_back.Models;

namespace Ocr_back.AuthHandlers;

public class TokenAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly AuthDbContext _db;
    private readonly UserManager<User> _userManager;

    public TokenAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, 
        ILoggerFactory logger, UrlEncoder encoder, AuthDbContext db, UserManager<User> userManager) 
        : base(options, logger, encoder)
    {
        _db = db;
        _userManager = userManager;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization") || !Request.Headers["Authorization"].ToString().StartsWith("Bearer"))
        {
            return AuthenticateResult.NoResult();
        }

        string bearer = Request.Headers["Authorization"].ToString().Substring("Bearer ".Length).Trim();
        
        var token = _db.AccessTokens.FirstOrDefault(x => x.Token == bearer);
        if (token is null || token.IsExpired()) 
            return AuthenticateResult.Fail("Invalid token.");

        var user = await _userManager.FindByIdAsync(token.UserId.ToString());
        if (user is null)
            return AuthenticateResult.Fail("Invalid token user.");
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("TokenType", token.TokenType.ToString()),
        };
        
        var identity = new ClaimsIdentity(claims, "Token");
        var principal = new ClaimsPrincipal(identity);
        
        return AuthenticateResult.Success(new AuthenticationTicket(principal, "Token"));
    }

    
}