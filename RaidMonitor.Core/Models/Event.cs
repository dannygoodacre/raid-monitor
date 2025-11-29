namespace RaidMonitor.Core.Models;

public class Event
{
    public required DateTime LoggedAt { get; init; }

    public required string Message { get; init; }

    public int? UserAcknowledgedId { get; init; }

    public DateTime? AcknowledgedAt { get; init; }

    public ICollection<int> UsersNotifiedIds { get; init; } = [];
}
