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
            .AddDbContext<ApplicationDbContext>(x => x.UseSqlite(connectionString))
            .AddDefaultIdentity<IdentityUser>(x => x.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IEventRepository, EventRepository>();

        return services;
    }
}
