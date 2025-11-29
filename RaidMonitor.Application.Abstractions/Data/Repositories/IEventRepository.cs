using RaidMonitor.Core.Models;

namespace RaidMonitor.Application.Abstractions.Data.Repositories;

public interface IEventRepository
{
    void Add(Event @event);
}
