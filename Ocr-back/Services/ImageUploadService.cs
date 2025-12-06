using Amazon.S3;
using Amazon.S3.Model;

namespace Ocr_back.Services;

public class ImageUploadService
{
    private readonly IAmazonS3 _s3;
    private readonly IConfiguration _configuration;

    public ImageUploadService(IAmazonS3 s3, IConfiguration configuration)
    {
        _s3 = s3;
        _configuration = configuration;
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        var bucketName = _configuration["S3:BucketName"];
        var fileExtension = Path.GetExtension(file.FileName);
        var fileKey = $"{Guid.NewGuid()}{fileExtension}";

        using var stream = file.OpenReadStream();

        var putRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = fileKey,
            InputStream = stream,
            ContentType = file.ContentType
        };

        await _s3.PutObjectAsync(putRequest);

        return fileKey;
    }

    public async Task<Stream> GetFileAsync(string key)
    {
        var bucketName = _configuration["S3:BucketName"];
        var getObjectRequest = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = key
        };
        
        var response = await _s3.GetObjectAsync(getObjectRequest);
        return response.ResponseStream;
    }

    public async Task<string> GeneratePresignedUrl(string key)
    {
        var bucketName = _configuration["S3:BucketName"];
        var getPreSignedUrlRequest = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = key,
            Expires = DateTime.UtcNow.AddHours(1)
        };
        
        return await _s3.GetPreSignedURLAsync(getPreSignedUrlRequest);
    }
}









