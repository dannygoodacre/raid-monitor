using RaidMonitor.Data.Entities;

namespace RaidMonitor.Data.Extensions;

internal static class EventExtensions
{
    public static RaidMonitor.Core.Models.Event ToModel(this Event value)
        => new()
        {
            LoggedAt = value.LoggedAt,
            Message = value.Message,
            UserAcknowledgedId = value.UserAcknowledgedId,
            AcknowledgedAt = value.AcknowledgedAt,
            UsersNotifiedIds = value.UsersNotifiedIds
        };

    public static Event ToEntity(this RaidMonitor.Core.Models.Event value)
        => new()
        {
            LoggedAt = value.LoggedAt,
            Message = value.Message,
            UserAcknowledgedId = value.UserAcknowledgedId,
            AcknowledgedAt = value.AcknowledgedAt,
        };
}
