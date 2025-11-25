using Microsoft.EntityFrameworkCore;
using RaidMonitor.Application.Abstractions.Data.Repositories;
using RaidMonitor.Core.Entities;
using RaidMonitor.Data.Extensions;

namespace RaidMonitor.Data.Repositories;

public class UserRepository(ApplicationContext context) : IUserRepository
{
    public Task<List<User>> GetUsersAsync(CancellationToken cancellationToken)
        => context.Users
            .Select(x => x.ToUser())
            .ToListAsync(cancellationToken);
}
