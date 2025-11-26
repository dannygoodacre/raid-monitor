namespace RaidMonitor.Application.Services;

internal sealed class FileService : IFileService
{
    public async Task<string[]> ReadProcMdstatAsync(CancellationToken cancellationToken)
        => await File.ReadAllLinesAsync("/proc/mdstat", cancellationToken);
}

internal interface IFileService
{
    public Task<string[]> ReadProcMdstatAsync(CancellationToken cancellationToken);
}
