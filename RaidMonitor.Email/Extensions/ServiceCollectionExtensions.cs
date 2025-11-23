using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RaidMonitor.Application.Abstractions.Services;
using Resend;

// ReSharper disable once CheckNamespace
namespace RaidMonitor.Email;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<ResendClient>();

        services.Configure<ResendClientOptions>(x =>
        {
            x.ApiToken = configuration["Email:ApiToken"] ?? throw new InvalidOperationException("ApiToken is missing");
        });

        services.AddTransient<IEmailSender, EmailSender>();

        services.AddTransient<IResend, ResendClient>();

        services.AddScoped<IEmailService, EmailSender>();

        return services;
    }
}
