using System.Text;
using System.Text.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Ocr_back.Controllers.OCR;
using Ocr_back.Data;
using Ocr_back.Services;
using OllamaSharp;
using OllamaSharp.Models;

namespace Ocr_back.Consumers;

public class OCRConsumer : IConsumer<OCREvent>
{
    private readonly OllamaApiClient _ollamaApiClient;
    private readonly ImageUploadService _imageUploadService;
    private readonly AppDataContext _db;

    public OCRConsumer(OllamaApiClient ollamaApiClient, ImageUploadService imageUploadService, AppDataContext db)
    {
        _ollamaApiClient = ollamaApiClient;
        _imageUploadService = imageUploadService;
        _db = db;
    }
    public async Task Consume(ConsumeContext<OCREvent> context)
    {
        _ollamaApiClient.SelectedModel = "llava-phi3";
        // // 2. Create Chat helper
        // var chat = new Chat(_ollamaApiClient);
        
        // get image from s3
        var imageStream = await _imageUploadService.GetFileAsync(context.Message.ImageUrl);
        
        //
        var memoryStream = new MemoryStream();
        await imageStream.CopyToAsync(memoryStream);
        var img = Convert.ToBase64String(memoryStream.ToArray());


        var gerReq = new GenerateRequest()
        {
            Prompt = "Extract all visible text from this image and return it as JSON. " +
                      "Use this format: { \"extracted_text\": \"all text here\", \"text_blocks\": [\"block 1\", \"block 2\"], \"confidence\": \"high/medium/low\" }. " +
                      "Return ONLY valid JSON, no markdown or extra text.",
            Images = [img],
            Stream = false,
            Model = "llava",
            Format = "json"
        };

        Console.WriteLine("Before GenerateAsync");
        var res = _ollamaApiClient.GenerateAsync(gerReq);
        string fullResponse = "";

        await foreach (var chunk in res)
        {
            fullResponse += chunk.Response;
        }

        var process = await _db.ImageProcesses
            .FirstOrDefaultAsync(x => x.Id == context.Message.ProcessId);

        if (process == null)
        {
            return;
        }
        
        process.ExtractedText = ExtractTextFromJson(fullResponse);
        _db.ImageProcesses.Update(process);
        await _db.SaveChangesAsync();
    }
    
    static string ExtractTextFromJson(string jsonResponse)
    {
        try
        {
            var jsonDoc = JsonDocument.Parse(jsonResponse);
            var root = jsonDoc.RootElement;
            
            // Get extracted_text field
            if (root.TryGetProperty("extracted_text", out var textElement))
            {
                return textElement.GetString() ?? "";
            }
            
            return "";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"JSON Parse Error: {ex.Message}");
            return "";
        }
    }
}