using MassTransit;
using System.Net.Mime;
using Microsoft.Net.Http.Headers;
using SixLabors.ImageSharp.Formats.Gif;
using SuperTracker.PixelApi.Settings;
using SuperTracker.PixelApi.Services.PixelTrackPublisher;
using SuperTracker.PixelApi.Services.ImageService;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddSingleton<IImageService, ImageService>();
builder.Services.AddScoped<IPixelTrackPublisherService, PixelTrackPublisherService>();

// Configure MassTransit with RabbitMQ using settings from configuration.
builder.Services.AddMassTransit(conf => 
{
    conf.UsingRabbitMq((ctx, cfg) =>
    {
        RabbitMqSettings rabbitMqSettings = new();
        builder.Configuration.Bind(RabbitMqSettings.Section, rabbitMqSettings);

        cfg.Host(rabbitMqSettings.Host, h =>
        {
            h.Username(rabbitMqSettings.Username);
            h.Password(rabbitMqSettings.Password);
        });
    });
});

var app = builder.Build();

// Add global basic exception handling middleware
app.UseExceptionHandler(exceptionHandlerApp 
    => exceptionHandlerApp.Run(async context 
        => await Results.Problem()
                     .ExecuteAsync(context)));

app.MapGet("/track", async (
    IPixelTrackPublisherService pixelTrackPublisherService,
    IImageService imageService,
    HttpContext context) =>
{
    // Collect infromation
    var referrer = context.Request.Headers[HeaderNames.Referer];
    var userAgent = context.Request.Headers[HeaderNames.UserAgent];
    var ipAddress = context.Connection.RemoteIpAddress?.ToString();

    // Check if ipAddress is null, return an error if it is
    if (string.IsNullOrEmpty(ipAddress))
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsync("Error: IP address is not available.");
        return;
    }

    // Publish a new pixel track event
    await pixelTrackPublisherService.Publish(referrer, userAgent, ipAddress);
    
    // Create a transparent 1x1 pixel image
    var image = imageService.GenerateDummyImage(1);

    // Write the image to the response stream as a GIF
    using var outputStream = new MemoryStream();
    image.Save(outputStream, new GifEncoder());
    outputStream.Seek(0, SeekOrigin.Begin);
    
    // Set response content type and copy image data to response body
    context.Response.ContentType = MediaTypeNames.Image.Gif;
    await outputStream.CopyToAsync(context.Response.Body);
});

app.Run();


