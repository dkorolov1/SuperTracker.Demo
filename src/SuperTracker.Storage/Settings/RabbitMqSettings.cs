namespace SuperTracker.Storage.Settings;

public class RabbitMqSettings
{
    public const string Section = "RabbitMq";

    public string Host { get; init; } = null!;

    public string Username { get; init; } = null!;

    public string Password { get; init; } = null!;
}