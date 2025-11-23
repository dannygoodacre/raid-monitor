using Microsoft.Extensions.Logging;
using RaidMonitor.Core.Common;
using RaidMonitor.Core.Entities;
using RaidMonitor.Application.Abstractions.Data;
using RaidMonitor.Application.Abstractions.Data.Repositories;
using RaidMonitor.Application.Abstractions.Services;

namespace RaidMonitor.Application.Commands;

internal sealed class SendWarningEmailHandler(ILogger<SendWarningEmailHandler> logger,
                                              IEmailService emailService,
                                              IUserRepository userRepository,
                                              IEventRepository eventRepository,
                                              IApplicationDbContext context) : CommandHandler<SendWarningEmailCommand>(logger), ISendWarningEmail
{
    protected override string CommandName => "Send Warning Email";

    protected override async Task<Result> InternalExecuteAsync(SendWarningEmailCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Command '{Command}' started.", CommandName);

        var userEmails = await userRepository.GetUserEmailsAsync(cancellationToken);

        var blocksHtml = string.Join("<br>", command.BadBlocks.Select(b => $"<pre><code>{b}</code></pre>"));

        var message = $"Errors found in {command.BadBlocks.Count} RAIDs:<br>{blocksHtml}";

        foreach (var email in userEmails)
        {
            await emailService.SendEmailAsync(email, "RAID ISSUE DETECTED", message);
        }

        eventRepository.Add(new Event
        {
            CapturedAt = DateTime.UtcNow,
            Message = message,
            IsAcknowledged = false
        });

        const int expectedChanges = 1;

        var actualChanges = await context.SaveChangesAsync();

        if (expectedChanges != actualChanges)
        {
            logger.LogWarning("Command '{Command}' wrote an unexpected number of changes to the database: expected '{Expected}', actual '{Actual}'.", CommandName, expectedChanges, actualChanges);
        }

        logger.LogInformation("Command '{Command}' completed, sending an email to: {Recipients}.", CommandName, string.Join(", ", userEmails));

        return Result.Success();
    }

    public Task<Result> ExecuteAsync(List<string> badBlocks, CancellationToken cancellationToken = default)
        => base.ExecuteAsync(new SendWarningEmailCommand()
        {
            BadBlocks = badBlocks
        }, cancellationToken);
}

public class SendWarningEmailCommand : ICommand
{
    public required List<string> BadBlocks { get; init; }
}

public interface ISendWarningEmail
{
    Task<Result> ExecuteAsync(List<string> badBlocks, CancellationToken cancellationToken = default);
}
