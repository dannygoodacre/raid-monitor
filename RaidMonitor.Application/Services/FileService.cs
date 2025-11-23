namespace RaidMonitor.Application.Services;

internal sealed class FileService : IFileService
{
    public async Task<string[]> ReadMdstatAsync(CancellationToken cancellationToken)
        => await File.ReadAllLinesAsync("/proc/mdstat", cancellationToken);
}

internal interface IFileService
{
    public Task<string[]> ReadMdstatAsync(CancellationToken cancellationToken);
}
