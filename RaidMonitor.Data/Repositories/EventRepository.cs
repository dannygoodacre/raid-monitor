using RaidMonitor.Application.Abstractions.Data.Repositories;
using RaidMonitor.Core.Models;
using RaidMonitor.Data.Extensions;

namespace RaidMonitor.Data.Repositories;

public class EventRepository(ApplicationContext context) : IEventRepository
{
    public void Add(Event @event) => context.Events.Add(@event.ToEntity());
}
