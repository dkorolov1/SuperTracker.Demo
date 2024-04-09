using Microsoft.Extensions.Options;
using SuperTracker.Storage.Settings;

namespace SuperTracker.Storage.UnitTests.Services;

public class PixelTrackStorageServiceTests
{
    private readonly LogFileSettings _logFileSettings;
    private readonly IPixelTrackStorageService _pixelTrackStorageService;
    
    public PixelTrackStorageServiceTests()
    {
        var logFileOptions  = Options.Create(new LogFileSettings { Path = "test.log" });
        _logFileSettings = logFileOptions .Value;
        _pixelTrackStorageService = new PixelTrackStorageService(logFileOptions);
    }

    [Fact]
    public async Task Store_Writes_To_File_And_Allows_Multiple_Concurrent_Requests()
    {
        // Arrange
        var notifications = new PixelTrackNotification[]
        {
            new ("https://www.adventureworlds.net/mystical-lands", "EnigmaVoyager/6.2 DragonScaleEdition", "127.0.0.1"),
            new (null, "EnchantedBrowser/5.8 (FairyOS X 11_2_9) GodzillaEdition", "127.0.0.2"),
            new ("https://www.fantasyrealms.com", null, "127.0.0.4"),
            new (null, null, "127.0.0.4")
        };

        // Act
        File.WriteAllText(_logFileSettings.Path, string.Empty);
        var tasks = notifications.Select(_pixelTrackStorageService.Store);
        await Task.WhenAll(tasks);

        // Assert
        Assert.True(File.Exists(_logFileSettings.Path));
        var lines = await File.ReadAllLinesAsync(_logFileSettings.Path);
        lines.Should().HaveCount(notifications.Length);
        var pairs = notifications.Zip(lines, (notification, line) => new { notification, line });
        pairs.Should().AllSatisfy(p => ValidatePixelTrackFileEntry(p.line, p.notification));
    }

    /// <summary>
    /// Validates the file entry for a PixelTrackNotification.
    /// </summary>
    /// <param name="entry">The file entry to validate.</param>
    /// <param name="notification">The PixelTrackNotification associated with the file entry.</param>
    private static void ValidatePixelTrackFileEntry(string entry, PixelTrackNotification notification) =>
        entry.Should().ContainAll(
                DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm"), //Accuracy to the minute.
                $"{notification.Referrer ?? "null"}|{notification.UserAgent ?? "null"}|{notification.IpAdress}");
}