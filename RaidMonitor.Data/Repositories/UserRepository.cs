using Microsoft.EntityFrameworkCore;
using RaidMonitor.Application.Abstractions.Data.Repositories;

namespace RaidMonitor.Data.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public Task<List<string>> GetUserEmailsAsync(CancellationToken cancellationToken)
        => context.Users
            .Where(x => !string.IsNullOrWhiteSpace(x.Email))
            .Select(x => x.Email!)
            .ToListAsync(cancellationToken);
}
