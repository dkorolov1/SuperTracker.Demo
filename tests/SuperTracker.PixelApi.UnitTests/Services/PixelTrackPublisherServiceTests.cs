using MassTransit;
using SuperTracker.PixelApi.Services.PixelTrackPublisher;

namespace SuperTracker.PixelApi.UnitTests.Services;

public class PixelTrackPublisherServiceTests
{
    private readonly IPixelTrackPublisherService _pixelTrackPublisherService;
    private readonly ILogger<PixelTrackPublisherService> _mockLogger;
    private readonly IPublishEndpoint _mockPublishEndpoint;

    public PixelTrackPublisherServiceTests()
    {
        _mockLogger = Substitute.For<ILogger<PixelTrackPublisherService>>();
        _mockPublishEndpoint = Substitute.For<IPublishEndpoint>();
        _pixelTrackPublisherService = new PixelTrackPublisherService(_mockLogger, _mockPublishEndpoint);
    }

    [Fact]
    public async Task Publish_ShouldLogAndPublishNotification()
    {
        // Arrange
        var referrer = "https://www.adventureworlds.net/mystical-lands";
        var userAgent = "EnchantedBrowser/5.0 (FairyOS X 11_2_3) DragonEdition";
        var ipAddress = "127.0.0.1";

        // Act
        await _pixelTrackPublisherService.Publish(referrer, userAgent, ipAddress);

        // Assert
        _mockLogger.Received(1).LogInformation(
            $"Publishing a new PixelTrackNotification. Referrer: {referrer}, UserAgent: {userAgent}, IpAdress: {ipAddress}");
        await _mockPublishEndpoint.Received(1).Publish(Arg.Is<PixelTrackNotification>(
            n => n.Referrer == referrer && n.UserAgent == userAgent && n.IpAdress == ipAddress));
    }
}