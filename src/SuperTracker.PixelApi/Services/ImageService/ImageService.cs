using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace SuperTracker.PixelApi.Services.ImageService;

public class ImageService(ILogger<ImageService> logger) : IImageService
{
    /// <inheritdoc/>
    public Image<Rgba32> GenerateDummyImage(int size)
    {
        logger.LogInformation($"Generating a new dummy transparent image of size {size}x{size}");
        
        var image = new Image<Rgba32>(size, size);
        image.Mutate(ctx => ctx.BackgroundColor(Color.Transparent));

        return image;
    }
}