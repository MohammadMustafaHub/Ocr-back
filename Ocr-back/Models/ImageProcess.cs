namespace Ocr_back.Models;

public class ImageProcess
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ImagePath { get; set; }
    public string? ExtractedText { get; set; }
    public string Hash { get; set; }
}