using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RaidMonitor.Application.Extensions;
using RaidMonitor.Application.Services;

// ReSharper disable once CheckNamespace
namespace RaidMonitor.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddCommandHandlers();

        services.AddQueryHandlers();

        services.AddScoped<IFileService, FileService>();

        return services;
    }

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        var handlerTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(x =>
                x is { IsAbstract: false, IsClass: true } && x.InheritsFromCommandHandler());

        foreach (var handlerType in handlerTypes)
        {
            var interfaces = handlerType.GetInterfaces();

            foreach (var serviceType in interfaces)
            {
                services.AddScoped(serviceType, handlerType);
            }
        }

        return services;
    }

    private static IServiceCollection AddQueryHandlers(this IServiceCollection services)
    {
        var handlerTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(x =>
                x is { IsAbstract: false, IsClass: true } && x.InheritsFromQueryHandler());

        foreach (var handlerType in handlerTypes)
        {
            var interfaces = handlerType.GetInterfaces();

            foreach (var serviceType in interfaces)
            {
                services.AddScoped(serviceType, handlerType);
            }
        }

        return services;
    }
}
