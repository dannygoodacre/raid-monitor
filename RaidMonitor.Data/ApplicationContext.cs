using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using RaidMonitor.Core.Entities;
using RaidMonitor.Application.Abstractions.Data;

namespace RaidMonitor.Data;

public class ApplicationContext(DbContextOptions<ApplicationContext> options) : IdentityDbContext(options), IApplicationContext
{
    public DbSet<Event> Events { get; set; }

    public Task<int> SaveChangesAsync() => base.SaveChangesAsync();
}

internal class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
{
    public ApplicationContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "RaidMonitor.Web"))
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
        optionsBuilder.UseSqlite(configuration.GetConnectionString("DefaultConnection"));

        return new ApplicationContext(optionsBuilder.Options);
    }
}
