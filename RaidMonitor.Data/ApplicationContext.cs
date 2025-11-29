using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using RaidMonitor.Application.Abstractions.Data;
using RaidMonitor.Data.Entities;

namespace RaidMonitor.Data;

public class ApplicationContext : IdentityDbContext<IdentityUser<int>, IdentityRole<int>, int>, IApplicationContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }

    public DbSet<Event> Events { get; set; }

    public Task<int> SaveChangesAsync() => base.SaveChangesAsync();
}

internal class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
{
    private SqliteConnection _inMemoryConnection = null!;

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

    public ApplicationContext CreateInMemoryDbContext()
    {
        _inMemoryConnection = new SqliteConnection("DataSource=:memory:");
        _inMemoryConnection.Open();

        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseSqlite(_inMemoryConnection)
            .Options;

        var context = new ApplicationContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public void DestroyInMemoryDbContext(ApplicationContext context)
    {
        context.Dispose();

        _inMemoryConnection.Close();
        _inMemoryConnection.Dispose();
    }
}
