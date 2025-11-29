using RaidMonitor.Core.Models;

namespace RaidMonitor.Application.Abstractions.Data.Repositories;

public interface IUserRepository
{
    public Task<List<User>> GetAllAsync(CancellationToken cancellationToken);
}
