using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RaidMonitor.Application.Abstractions.Data;
using RaidMonitor.Application.Abstractions.Data.Repositories;
using RaidMonitor.Data.Repositories;

// ReSharper disable once CheckNamespace
namespace RaidMonitor.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services
            .AddDbContext<ApplicationContext>(x => x.UseSqlite(connectionString))
            .AddDefaultIdentity<IdentityUser>(x => x.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationContext>();

        services.AddScoped<IApplicationContext>(provider => provider.GetRequiredService<ApplicationContext>());

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IEventRepository, EventRepository>();

        return services;
    }
}
