namespace SuperTracker.PixelApi.Services.PixelTrackPublisher;

public interface IPixelTrackPublisherService
{
    /// <summary>
    /// Publishes a PixelTrackNotification to the queue with the provided referrer, user agent, and IP address.
    /// </summary>
    /// <param name="referrer">The referrer URL</param>
    /// <param name="userAgent">The user agent of the client making the request.</param>
    /// <param name="ipAdress">The IP address of the client making the request.</param>
    Task Publish(string? referrer, string? userAgent, string ipAdress);
}