using Microsoft.EntityFrameworkCore;
using RaidMonitor.Application.Abstractions.Data.Repositories;
using RaidMonitor.Core.Models;
using RaidMonitor.Data.Extensions;

namespace RaidMonitor.Data.Repositories;

public class UserRepository(ApplicationContext context) : IUserRepository
{
    public Task<List<User>> GetAllAsync(CancellationToken cancellationToken)
        => context.Users
            .Select(x => x.ToModel())
            .ToListAsync(cancellationToken);
}
