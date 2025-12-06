using Microsoft.AspNetCore.Mvc;
using Ocr_back.Data;

namespace Ocr_back.Controllers.OCR;

[ApiController]
[Route("[controller]")]
public class HookController : Controller
{
    private readonly AppDataContext _db;

    public HookController(AppDataContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Post(HookRequest request)
    {
        
        return Ok();
    }
    
}


public class HookRequest
{
    public IFormFile Image { get; set; }
    public string WebhookUrl { get; set; }
    public string Key { get; set; }
}















