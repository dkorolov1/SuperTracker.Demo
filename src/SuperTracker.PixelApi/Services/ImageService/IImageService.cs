using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace SuperTracker.PixelApi.Services.ImageService;

public interface IImageService
{
    /// <summary>
    /// Generates a dummy transparent image with the specified size.
    /// </summary>
    /// <param name="size">The size of the dummy image.</param>
    /// <returns>The generated dummy image.</returns>
    Image<Rgba32> GenerateDummyImage(int size);
}