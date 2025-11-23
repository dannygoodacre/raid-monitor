namespace RaidMonitor.Application.Abstractions.Data.Repositories;

public interface IUserRepository
{
    public Task<List<string>> GetUserEmailsAsync(CancellationToken cancellationToken = default);
}
