using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Ocr_back.Data;
using Ocr_back.Models;
using Ocr_back.Services;
using OllamaSharp;

namespace Ocr_back.Controllers.OCR;

[ApiController]
[Route("[controller]")]
[Authorize]
public class OcrController : Controller
{
    private readonly ImageUploadService _imageUploadService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly AppDataContext _db;

    public OcrController(ImageUploadService imageUploadService, IPublishEndpoint publishEndpoint, AppDataContext db)
    {
        _imageUploadService = imageUploadService;
        _publishEndpoint = publishEndpoint;
        _db = db;
    }
    [HttpPost]
    [EnableRateLimiting("OCR")]
    public async Task<IActionResult> Post(OCRImageRequest request)
    {

        var userId = Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value);
        var hash = GetFileHash(request.Image.OpenReadStream());
        
        // find by hash
        var res = _db.ImageProcesses.FirstOrDefault(x => x.Hash == hash && x.UserId == userId);
        if(res is not null)
            return Ok(new { Text = res.ExtractedText });
        
        string url = await _imageUploadService.UploadFileAsync(request.Image);
        var image = new ImageProcess()
        {
            Id = Guid.NewGuid(),
            Hash = hash,
            ImagePath = url,
            ExtractedText = null,
            UserId = userId

        };
        _db.ImageProcesses.Add(image);
        
        await _publishEndpoint.Publish<OCREvent>(new OCREvent()
        {
            ImageUrl = url,
            ProcessId = image.Id
        });
        
        await _db.SaveChangesAsync();
        
        return Accepted(new  { ProcessId = image.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Get(Guid id)
    {
        var userId = Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value);
        var image = _db.ImageProcesses.FirstOrDefault(x => x.Id == id && x.UserId == userId);
        if(image is null) 
            return NotFound();
        
        if(image.ExtractedText is not null)
            return Ok(new ExtractedTextResponse()
            {
                ImageUrl = await _imageUploadService.GeneratePresignedUrl(image.ImagePath),
                Text = image.ExtractedText,
                ProcessId = image.Id
            });

        return Accepted();
    }
    
    
    private static string GetFileHash(Stream stream)
    {
    
        byte[] hashBytes;
        using (var sha256 = SHA256.Create())
        {
            hashBytes = sha256.ComputeHash(stream);
        }

        
        return Encoding.UTF8.GetString(hashBytes);
    }
}


public class OCRImageRequest
{
    public IFormFile Image { get; set; }
}

public class OCREvent
{
    public string ImageUrl { get; set; }
    public Guid ProcessId { get; set; }
}


public class ExtractedTextResponse
{
    public string Text { get; set; }
    public Guid ProcessId { get; set; }
    public string ImageUrl { get; set; }
}







