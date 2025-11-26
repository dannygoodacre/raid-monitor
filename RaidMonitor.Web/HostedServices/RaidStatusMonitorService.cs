using Microsoft.Extensions.Options;
using RaidMonitor.Application.Commands;
using RaidMonitor.Application.Queries;
using RaidMonitor.Configuration;
using RaidMonitor.Configuration.Options;

namespace RaidMonitor.Web.HostedServices;

public class RaidStatusMonitorService(ILogger<RaidStatusMonitorService> logger,
                                      IOptions<ServiceOptions> options,
                                      IServiceScopeFactory scopeFactory) : BackgroundService
{
    private const string ServiceName = "RAID Status Monitor";

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();

            var getStatus = scope.ServiceProvider.GetRequiredService<IGetRaidStatus>();

            var getStatusResult = await getStatus.ExecuteAsync(cancellationToken);

            if (!getStatusResult.IsSuccess)
            {
                logger.LogError("Service '{Service}' could not validate the state of the system RAIDs.", ServiceName);

                await Task.Delay(TimeSpan.FromSeconds(options.Value.DelayInSeconds), cancellationToken);

                continue;
            }

            if (getStatusResult.Value.Count == 0)
            {
                await Task.Delay(TimeSpan.FromSeconds(options.Value.DelayInSeconds), cancellationToken);

                continue;
            }

            var sendWarningEmail = scope.ServiceProvider.GetRequiredService<ISendWarningEmail>();

            var sendResult = await sendWarningEmail.ExecuteAsync(getStatusResult.Value, cancellationToken);

            if (!sendResult.IsSuccess)
            {
                logger.LogError("Service '{Service}' could not send email.", ServiceName);
            }

            await Task.Delay(TimeSpan.FromSeconds(options.Value.DelayInSeconds), cancellationToken);
        }
    }
}
