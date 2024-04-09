namespace SuperTracker.Contracts.PixelTrack;

public record PixelTrackNotification(string? Referrer, string? UserAgent, string IpAdress)
{
    public override string ToString() =>
        $@"{DateTime.UtcNow:o}|{Referrer ?? "null"}|{UserAgent ?? "null"}|{IpAdress}";
}