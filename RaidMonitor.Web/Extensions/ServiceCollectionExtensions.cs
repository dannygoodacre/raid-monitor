using Microsoft.OpenApi;
using RaidMonitor.Web.HostedServices;

// ReSharper disable once CheckNamespace
namespace RaidMonitor.Web;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<RaidStatusMonitorService>();

        return services;
    }

    public static IServiceCollection AddWeb(this IServiceCollection services)
    {
        services.AddControllers();

        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddRazorPages();

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(x => x.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "RaidMonitor",
            Version = "v0.1"
        }));

        return services;
    }
}
