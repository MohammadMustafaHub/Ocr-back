using Ocr_back.Models;

namespace Ocr_back.Controllers.Auth.Dtos;

public class UserResponse
{
    public string Email { get; set; }
    public string Role { get; set; }
    public Guid Id { get; set; }

}