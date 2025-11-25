using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RaidMonitor.Application.Services;
using RaidMonitor.Configuration.Options;
using RaidMonitor.Core.Common;

namespace RaidMonitor.Application.Queries;

internal sealed class ValidateRaidStatusHandler(ILogger<ValidateRaidStatusHandler> logger,
                                                IOptions<RaidIssueOptions> options,
                                                IFileService fileService) : QueryHandler<ValidateRaidStatusQuery, List<string>>(logger), IValidateRaidStatus
{
    protected override string QueryName => "Validate RAID Status";

    protected override async Task<Result<List<string>>> InternalExecuteAsync(ValidateRaidStatusQuery command, CancellationToken cancellationToken)
    {
        var fileContent = await fileService.ReadMdstatAsync(cancellationToken);

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

        logger.LogInformation("Command '{Command}' found '{Count}' bad arrays: {BadArrays}.", QueryName, badArrays.Count, string.Join(Environment.NewLine, badArrays));

        return Result<List<string>>.Success(badArrays);
    }

    public Task<Result<List<string>>> ExecuteAsync(CancellationToken cancellationToken) => base.ExecuteAsync(new ValidateRaidStatusQuery(), cancellationToken);
}

public class ValidateRaidStatusQuery : IQuery
{
}

public interface IValidateRaidStatus
{
    public Task<Result<List<string>>> ExecuteAsync(CancellationToken cancellationToken = default);
}
