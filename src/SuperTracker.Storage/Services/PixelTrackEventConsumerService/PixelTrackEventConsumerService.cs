using MassTransit;
using SuperTracker.Contracts.PixelTrack;
using SuperTracker.Storage.Services.PixelTrackStorageService;

namespace SuperTracker.Storage.Services.PixelTrackConsumerService;

public class PixelTrackEventConsumerService(
    IPixelTrackStorageService pixelTrackStorageService,
    ILogger<PixelTrackEventConsumerService> logger) : IConsumer<PixelTrackNotification>
{
    /// <summary>
    /// Consumes a PixelTrackNotification message from a message queue.
    /// </summary>
    /// <param name="context">The ConsumeContext containing the PixelTrackNotification message.</param>
    public async Task Consume(ConsumeContext<PixelTrackNotification> context)
    {
        var message = context.Message;
        if (string.IsNullOrEmpty(message.IpAdress))
        {
            logger.LogError("Received a PixelTrackNotification with empty or null IP address.");
            throw new ArgumentException("IP address cannot be empty or null.", nameof(message.IpAdress));
        }
        logger.LogInformation($"Received a new PixelTrackNotification: {message}");
        await pixelTrackStorageService.Store(message);
    }
}