using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ocr_back.Controllers.Auth.Dtos;
using Ocr_back.Data;
using Ocr_back.Models;

namespace Ocr_back.Controllers.Auth;


[ApiController]
[Route("[controller]")]
public class TokensController : Controller
{
    private readonly AuthDbContext _db;
    private readonly UserManager<User> _userManager;

    public TokensController(AuthDbContext db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> GenerateApi(GenerateTokenRequest request)
    {
        if (User.Claims.FirstOrDefault(x => x.Type == "TokenType")!.Value != "Auth")
        {
            return Unauthorized();
        } 
        
        var userId = Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value);
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if(user is null) 
            return BadRequest();

        var token = AccessToken.Create(user, TokenType.Access, null, request.KeyName);
        _db.AccessTokens.Add(token);
        await _db.SaveChangesAsync();
        
        return Ok(new TokenResponse()
        {
            AccessToken = token.Token
        });
    }
    // get tokens
    [HttpGet]
    public async Task<IActionResult> GetTokens()
    {
        if (User.Claims.FirstOrDefault(x => x.Type == "TokenType")!.Value != "Auth")
        {
            return Unauthorized();
        } 
        var userId = Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value);
        var tokens = await _db.AccessTokens.Where(x => x.TokenType == TokenType.Access).ToListAsync();
        
        return Ok(tokens.Select(x => new ApiTokensResponse()
        {
            Name = x.TokenName!
        }));
    }
    
}

















