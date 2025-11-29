using Microsoft.AspNetCore.Identity;

namespace RaidMonitor.Data.Entities;

public class Event
{
    public int Id { get; set; }

    public required DateTime LoggedAt { get; set; }

    public required string Message { get; set; }

    public int? UserAcknowledgedId { get; set; }

    public IdentityUser<int>? UserAcknowledged { get; set; }

    public DateTime? AcknowledgedAt { get; set; }

    public ICollection<int> UsersNotifiedIds { get; set; } = [];

    public ICollection<IdentityUser<int>> UsersNotified { get; set; } = [];
}
