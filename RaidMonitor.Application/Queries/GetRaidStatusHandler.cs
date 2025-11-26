using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RaidMonitor.Application.Services;
using RaidMonitor.Configuration.Options;
using RaidMonitor.Core.Common;

namespace RaidMonitor.Application.Queries;

internal sealed class GetRaidStatusHandler(ILogger<GetRaidStatusHandler> logger,
                                           IOptions<RaidIssueOptions> options,
                                           IFileService fileService) : QueryHandler<GetRaidStatusQuery, List<string>>(logger), IGetRaidStatus
{
    protected override string QueryName => "Get RAID Status";

    protected override async Task<Result<List<string>>> InternalExecuteAsync(GetRaidStatusQuery command, CancellationToken cancellationToken)
    {
        var fileContent = await fileService.ReadProcMdstatAsync(cancellationToken);

        var blocks = fileContent
            .SkipWhile(l => l.StartsWith("Personalities"))
            .TakeWhile(l => !l.StartsWith("unused devices"))
            .Aggregate(new List<string> { "" }, (list, line) =>
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    list.Add("");
                }
                else
                {
                    list[^1] += line + Environment.NewLine;
                }

                return list;
            })
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim())
            .ToList();

        var badArrays = blocks.Where(block => options.Value.Keywords.Any(block.Contains)).ToList();

        if (!badArrays.Any())
        {
            return Result<List<string>>.Success([]);
        }

        logger.LogInformation("Query '{Query}' found '{Count}' bad arrays: {BadArrays}", QueryName, badArrays.Count, Environment.NewLine + string.Join(Environment.NewLine, badArrays));

        return Result<List<string>>.Success(badArrays);
    }

    public Task<Result<List<string>>> ExecuteAsync(CancellationToken cancellationToken) => base.ExecuteAsync(new GetRaidStatusQuery(), cancellationToken);
}

public class GetRaidStatusQuery : IQuery
{
}

public interface IGetRaidStatus
{
    public Task<Result<List<string>>> ExecuteAsync(CancellationToken cancellationToken = default);
}
