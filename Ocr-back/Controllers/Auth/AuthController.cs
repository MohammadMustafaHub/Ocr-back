using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ocr_back.Controllers.Auth.Dtos;
using Ocr_back.Data;
using Ocr_back.Http;
using Ocr_back.Models;
using Ocr_back.Services;

namespace Ocr_back.Controllers.Auth;

[ApiController]
[Route("[controller]")]
public class AuthController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly TokenService _tokenService;
    private readonly AuthDbContext _db;

    public AuthController(UserManager<User> userManager, TokenService tokenService, AuthDbContext db)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _db = db;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var u = await _userManager.FindByEmailAsync(request.Email);
        if (u != null) return BadRequest(new ErrorResponse("Email", "EMAIL_USED"));

        var user = new User()
        {
            Email = request.Email,
            UserName = request.Email
        };
        
        var userRes = await _userManager.CreateAsync(user, request.Password);
        if (!userRes.Succeeded)
        {
            var errors = userRes.Errors.Select(x => new ErrorResponse(x.Code, x.Description));
            return BadRequest(errors);
        }
        
        await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "User"));

        var refresh = await _generateRefreshToken(user);
        var token = await _tokenService.GenerateJwtToken(user);
        return Ok(new TokenResponse()
        {
            AccessToken = token,
            RefreshToken = refresh.Token
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(RegisterRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return BadRequest(new ErrorResponse("Email", "INVALID_CREDENTIALS"));

        if (await _userManager.IsLockedOutAsync(user))
        {
            return BadRequest(new ErrorResponse("Account", "USER_LOCKED"));
        }
        
        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
        {
            await _userManager.AccessFailedAsync(user);
            return BadRequest(new ErrorResponse("Password", "INVALID_CREDENTIALS"));
        }
        
        await _userManager.ResetAccessFailedCountAsync(user);
        
        var refresh = await _generateRefreshToken(user);
        var token = await _tokenService.GenerateJwtToken(user);
        
        return Ok(new TokenResponse()
        {
            AccessToken = token,
            RefreshToken = refresh.Token,
        });
    }
    
    
    private async Task<RefreshToken> _generateRefreshToken(User user)
    {
        var token = RefreshToken.Create(user);
        _db.RefreshTokens.Add(token);
        await _db.SaveChangesAsync();
        return token;
    }
    
}














