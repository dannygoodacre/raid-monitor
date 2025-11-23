using RaidMonitor.Core.Entities;
using RaidMonitor.Application.Abstractions.Data.Repositories;

namespace RaidMonitor.Data.Repositories;

public class EventRepository(ApplicationDbContext context) : IEventRepository
{
    public void Add(Event @event) => context.Events.Add(@event);
}
