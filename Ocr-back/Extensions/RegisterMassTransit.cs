using MassTransit;
using Ocr_back.Consumers;

namespace Ocr_back.Extensions;

public static class RegisterMassTransit
{
    public static void AddMassTransitServices(this IServiceCollection services)
    {
        services.AddMassTransit(configure =>
        {
            configure.AddConsumer<OCRConsumer>();

            configure.UsingAmazonSqs((context, cfg) =>
            {
                cfg.Host("eu-central-1", h =>
                {
                });

                cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter("OCR", false));
            });
        });
    }
}