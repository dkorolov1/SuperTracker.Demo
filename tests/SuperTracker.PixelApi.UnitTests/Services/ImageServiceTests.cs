using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SuperTracker.PixelApi.Services.ImageService;

namespace SuperTracker.PixelApi.UnitTests.Services;

public class ImageServiceTests
{
    private readonly IImageService imageService;
    private readonly ILogger<ImageService> _mockLogger;

    public ImageServiceTests()
    {
        _mockLogger = Substitute.For<ILogger<ImageService>>();
        imageService = new ImageService(_mockLogger);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void GenerateDummyImage_ShouldGenerateTransparentImage_AndLogInformation(int imageSize)
    {
        // Act
        Image<Rgba32> result = imageService.GenerateDummyImage(imageSize);

        // Assert
        result.Should().NotBeNull();
        result.Width.Should().Be(imageSize);
        result.Height.Should().Be(imageSize);

        var transparentPixelsCount = Enumerable.Range(0, result.Width)
            .SelectMany(x => Enumerable.Range(0, result.Height)
                .Select(y => result[x, y]))
            .Count(pixel => pixel.A == 0);

        transparentPixelsCount.Should().Be(imageSize * imageSize); // All pixels should be transparent
        _mockLogger.Received(1).LogInformation($"Generating a new dummy transparent image of size {imageSize}x{imageSize}");
    }
}