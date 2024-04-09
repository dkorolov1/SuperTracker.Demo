using MassTransit;
using SuperTracker.Contracts.PixelTrack;

namespace SuperTracker.PixelApi.Services.PixelTrackPublisher;

public class PixelTrackPublisherService(
    ILogger<PixelTrackPublisherService> logger,
    IPublishEndpoint publishEndpoint) : IPixelTrackPublisherService
{
    /// <inheritdoc/>
    public async Task Publish(string? referrer, string? userAgent, string ipAdress)
    {
        logger.LogInformation($"Publishing a new PixelTrackNotification. Referrer: {referrer}, UserAgent: {userAgent}, IpAdress: {ipAdress}");
        await publishEndpoint.Publish(new PixelTrackNotification(referrer, userAgent, ipAdress));
    }
}