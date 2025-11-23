using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RaidMonitor.Core.Entities;
using RaidMonitor.Application.Abstractions.Data;

namespace RaidMonitor.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options), IApplicationDbContext
{
    public DbSet<Event> Events { get; set; }

    public Task<int> SaveChangesAsync() => base.SaveChangesAsync();

    public Task MigrateAsync() => Database.MigrateAsync();
}
