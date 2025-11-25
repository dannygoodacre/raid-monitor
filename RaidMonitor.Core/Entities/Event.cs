namespace RaidMonitor.Core.Entities;

public class Event
{
    public int Id { get; set; }

    public required DateTime CapturedAt { get; set; }

    public required string Message { get; set; }

    public required bool IsAcknowledged { get; set; }

    public ICollection<User> UsersSendTo { get; set; } = [];
}
