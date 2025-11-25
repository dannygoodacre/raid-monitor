using RaidMonitor.Core.Entities;

namespace RaidMonitor.Application.Abstractions.Data.Repositories;

public interface IUserRepository
{
    public Task<List<User>> GetUsersAsync(CancellationToken cancellationToken);
}
