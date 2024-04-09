using Microsoft.Extensions.Options;
using SuperTracker.Contracts.PixelTrack;
using SuperTracker.Storage.Settings;

namespace SuperTracker.Storage.Services.PixelTrackStorageService;

public class PixelTrackStorageService(
    IOptions<LogFileSettings> logFileOptions) : IPixelTrackStorageService
{
    private static readonly SemaphoreSlim _semaphore = new(1);
    private readonly LogFileSettings _logFileSettings = logFileOptions.Value;

    /// <inheritdoc/>
    public async Task Store(PixelTrackNotification notification)
    {
        await _semaphore.WaitAsync();
        try
        {
            // Append data to the file
            using StreamWriter writer = File.AppendText(_logFileSettings.Path);
            await writer.WriteLineAsync(notification.ToString());
        }
        finally
        {
            _semaphore.Release();
        }
    }
}