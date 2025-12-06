using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Ocr_back.Data;
using Ocr_back.Services;
using OllamaSharp;

namespace Ocr_back.Extensions;

public static class RegisterAppServices
{
    public static void AddAppServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IAmazonS3>(op =>
        {
            var config = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.EUCentral1,
            };
            
            return new AmazonS3Client(config);
        });

        services.AddScoped<ImageUploadService>();

        services.AddDbContext<AppDataContext>(op =>
        {
            op.UseNpgsql(config.GetConnectionString("AppConnection"));
        });

        services.AddSingleton<OllamaApiClient>(op =>
        {
            return new OllamaApiClient(new Uri(config["OllamaApiUrl"]!));
        });

    }
}










