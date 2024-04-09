using MassTransit;
using NSubstitute;
using Microsoft.Extensions.Logging.Testing;
using SuperTracker.Storage.Services.PixelTrackConsumerService;

namespace SuperTracker.Storage.UnitTests.Services;

public class PixelTrackEventConsumerServiceTests
{
    private readonly PixelTrackEventConsumerService _pixelTrackConsumerService;
    private readonly FakeLogger<PixelTrackEventConsumerService> _fakeLogger;
    private readonly ConsumeContext<PixelTrackNotification> _mockConsumeContext;
    private readonly IPixelTrackStorageService _mockPixelTrackStorageService;

    public PixelTrackEventConsumerServiceTests()
    {
        _fakeLogger = new FakeLogger<PixelTrackEventConsumerService>();
        _mockConsumeContext = Substitute.For<ConsumeContext<PixelTrackNotification>>();
        _mockPixelTrackStorageService = Substitute.For<IPixelTrackStorageService>();
        _pixelTrackConsumerService = new PixelTrackEventConsumerService(
            _mockPixelTrackStorageService, _fakeLogger);
    }

    [Theory]
    [MemberData(nameof(ListPixelTrackNotifications))] // The number of concurrent tasks can be adjusted if needed
    public async Task Consume_ShouldLog_And_Store(PixelTrackNotification notification)
    {
        //Arrange
        _mockConsumeContext.Message.Returns(notification);

        // Act
        await _pixelTrackConsumerService.Consume(_mockConsumeContext);

        // Assert
        _fakeLogger.Collector.Count.Should().Be(1);
        ValidatePixelTrackLogEntry(_fakeLogger.Collector.LatestRecord.Message, notification);
        await _mockPixelTrackStorageService.Received(1).Store(notification);
    }

    [Theory]
    [MemberData(nameof(ListPixelTrackNotificationsWithEmptyIpAddress))]
    public async Task Consume_WithEmptyIpAddress_ShouldLogError_AndNotStore(PixelTrackNotification notification)
    {
        // Arrange
        _mockConsumeContext.Message.Returns(notification);

        // Act
        Func<Task> act = async () => await 
            _pixelTrackConsumerService.Consume(_mockConsumeContext);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("IP address cannot be empty or null.*");
        _fakeLogger.Collector.Count.Should().Be(1);
        _fakeLogger.Collector.LatestRecord.Message.Should().Be("Received a PixelTrackNotification with empty or null IP address.");
        await _mockPixelTrackStorageService.DidNotReceive().Store(Arg.Any<PixelTrackNotification>());
    }
    
    /// <summary>
    /// Provides test data for pixel track notifications with various combinations of properties.
    /// </summary>
    public static TheoryData<PixelTrackNotification> ListPixelTrackNotifications() =>
        new() {
            new ("https://www.adventureworlds.net/mystical-lands", "EnigmaVoyager/6.2 (FairyOS X 11_2_9) DragonScaleEdition", "127.0.0.1"),
            new (null, "EnchantedBrowser/5.8 (FairyOS X 11_2_9) GodzillaEdition", "127.0.0.2"),
            new ("https://www.fantasyrealms.com", null, "127.0.0.4"),
            new (null, null, "127.0.0.4")
        };

    /// <summary>
    /// Provides test data for pixel track notifications with empty or null IP addresses.
    /// </summary>
    public static TheoryData<PixelTrackNotification> ListPixelTrackNotificationsWithEmptyIpAddress() =>
        new() {
            new ("https://www.adventureworlds.net/mystical-lands", "EnigmaVoyager/6.2 (FairyOS X 11_2_9) DragonScaleEdition", ""),
            #pragma warning disable CS8625
            new ("https://www.adventureworlds.net/mystical-lands", "EnigmaVoyager/6.2 DragonScaleEdition", null)
            #pragma warning restore CS8625
        };

    /// <summary>
    /// Validates the log entry for a PixelTrackNotification.
    /// </summary>
    /// <param name="logEntry">The log entry to validate.</param>
    /// <param name="notification">The PixelTrackNotification associated with the log entry.</param>
    private static void ValidatePixelTrackLogEntry(string logEntry, PixelTrackNotification notification) =>
        logEntry.Should().ContainAll(
                DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm"), //Accuracy to the minute.
                $"{notification.Referrer ?? "null"}|{notification.UserAgent ?? "null"}|{notification.IpAdress}");
}