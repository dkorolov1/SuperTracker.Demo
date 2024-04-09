using SuperTracker.Contracts.PixelTrack;

namespace SuperTracker.Storage.Services.PixelTrackStorageService;

public interface IPixelTrackStorageService
{
    /// <summary>
    /// Stores a PixelTrackNotification to a log file, ensuring thread safety.
    /// </summary>
    /// <param name="notification">The PixelTrackNotification to store.</param>
    Task Store(PixelTrackNotification notification);
}