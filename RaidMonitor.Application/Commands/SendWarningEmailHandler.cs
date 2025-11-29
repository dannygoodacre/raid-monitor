using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RaidMonitor.Core.Common;
using RaidMonitor.Application.Abstractions.Data;
using RaidMonitor.Application.Abstractions.Data.Repositories;
using RaidMonitor.Application.Abstractions.Services;
using RaidMonitor.Configuration.Options;
using RaidMonitor.Core.Models;

namespace RaidMonitor.Application.Commands;

internal sealed class SendWarningEmailHandler(ILogger<SendWarningEmailHandler> logger,
                                              IOptions<EmailOptions> options,
                                              IEmailService emailService,
                                              IUserRepository userRepository,
                                              IEventRepository eventRepository,
                                              IApplicationContext context) : CommandHandler<SendWarningEmailCommand>(logger), ISendWarningEmail
{
    protected override string CommandName => "Send Warning Email";

    protected override async Task<Result> InternalExecuteAsync(SendWarningEmailCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Command '{Command}' started.", CommandName);

        var blocksHtml = string.Join("<br>", command.BadArrays.Select(b => $"<pre><code>{b}</code></pre>"));

        var message = $"Issues found in {command.BadArrays.Count} arrays:<br>{blocksHtml}";

        List<string> emails;

        var users = await userRepository.GetAllAsync(cancellationToken);

        if (users.Count == 0)
        {
            logger.LogWarning("Command '{Command}' found no users to contact; using backup email '{BackupEmail}' instead.", CommandName, options.Value.BackupEmail);

            emails = [ options.Value.BackupEmail ];
        }
        else
        {
            emails = users.Select(x => x.Email).ToList();
        }

        foreach (var email in emails)
        {
            await emailService.SendEmailAsync(email, options.Value.Subject, message);
        }

        var foo = users.Select(x => x.Id).ToList();

        eventRepository.Add(new Event
        {
            LoggedAt = DateTime.UtcNow,
            Message = message,
            UsersNotifiedIds = users.Select(x => x.Id).ToList(),
        });

        const int expectedChanges = 1;

        var actualChanges = await context.SaveChangesAsync();

        if (expectedChanges != actualChanges)
        {
            logger.LogWarning("Command '{Command}' wrote an unexpected number of changes to the database: expected '{Expected}', actual '{Actual}'.", CommandName, expectedChanges, actualChanges);
        }

        logger.LogInformation("Command '{Command}' completed, sending an email to: {Recipients}.", CommandName, string.Join(", ", emails));

        return Result.Success();
    }

    public Task<Result> ExecuteAsync(List<string> badArrays, CancellationToken cancellationToken)
        => base.ExecuteAsync(new SendWarningEmailCommand
        {
            BadArrays = badArrays
        }, cancellationToken);
}

public class SendWarningEmailCommand : ICommand
{
    public required List<string> BadArrays { get; init; }
}

public interface ISendWarningEmail
{
    Task<Result> ExecuteAsync(List<string> badArrays, CancellationToken cancellationToken = default);
}
