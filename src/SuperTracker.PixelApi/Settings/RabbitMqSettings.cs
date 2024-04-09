namespace SuperTracker.PixelApi.Settings;

public class RabbitMqSettings
{
    public const string Section = "RabbitMq";

    public string Host { get; init; } = null!;

    public string Username { get; init; } = null!;

    public string Password { get; init; } = null!;
}