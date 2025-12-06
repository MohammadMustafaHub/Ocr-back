using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Ocr_back.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddRateLimiter(op =>
{
    op.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    op.AddFixedWindowLimiter("OCR", fop =>
    {
        fop.PermitLimit = 1;
        fop.Window = TimeSpan.FromMinutes(1);
        fop.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        fop.QueueLimit = 0;
    });
});

builder.Services.AddCors(op =>
{
    op.AddPolicy("AllowFrontEnd", bld =>
    {
        bld.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddAuthServices(builder.Configuration);
builder.Services.AddAppServices(builder.Configuration);
builder.Services.AddMassTransitServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontEnd");
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();
