using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ocr_back.Controllers.Auth.Dtos;
using Ocr_back.Models;

namespace Ocr_back.Controllers.Auth;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly UserManager<User> _userManager;

    public UserController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }
    
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<User>> Get()
    {
        var userId = Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value);
        
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return NotFound();
        
        var claims = await _userManager.GetClaimsAsync(user);
        var role = claims.First(x => x.Type == ClaimTypes.Role);

        return Ok(new UserResponse()
        {
            Email = user.Email!,
            Role = role.Value,
            Id = user.Id
        });
    }
}









