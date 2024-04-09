namespace SuperTracker.Storage.Settings;

public class LogFileSettings
{
    public const string Section = "LogFile";

    public string Path { get; init; } = "tmp/visits.log";
}