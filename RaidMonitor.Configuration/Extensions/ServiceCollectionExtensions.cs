using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using RaidMonitor.Configuration.Options;

// ReSharper disable once CheckNamespace
namespace RaidMonitor.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<EmailOptions>()
            .Bind(configuration.GetSection("Email"))
            .ValidateOnStart();

        services
            .AddOptions<ServiceOptions>()
            .Bind(configuration.GetSection("Services"))
            .ValidateOnStart();

        services
            .AddOptions<RaidIssueOptions>()
            .Bind(configuration.GetSection("RaidIssueKeywords"))
            .ValidateOnStart();

        return services;
    }
}
