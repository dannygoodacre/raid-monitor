using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RaidMonitor.Core.Models;
using RaidMonitor.Data.Repositories;

namespace RaidMonitor.Data.Tests.Repositories;

[TestFixture]
public class EventRepositoryTests : TestBase
{
    private ApplicationContext _context;

    private EventRepository _repository;

    [SetUp]
    public void SetUp()
    {
        var factory = new ApplicationContextFactory();

        _context = factory.CreateInMemoryDbContext();

        _repository = new EventRepository(_context);
    }

    [Test]
    public async Task Add_ShouldAddEvent()
    {
        // Arrange
        var user = new IdentityUser<int>
        {
            Id = 123
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        var @event = new Event
        {
            LoggedAt = new DateTime(2025, 12, 10),
            Message = "Test Message",
            UserAcknowledgedId = 123
        };

        // Act
        _repository.Add(@event);

        await _context.SaveChangesAsync();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_context.Events.Count, Is.EqualTo(1));

            var eventEntity = _context.Events.Single();

            Assert.That(eventEntity.LoggedAt, Is.EqualTo(@event.LoggedAt));
            Assert.That(eventEntity.Message, Is.EqualTo(@event.Message));
            Assert.That(eventEntity.UserAcknowledgedId, Is.EqualTo(@event.UserAcknowledgedId));
        }
    }
}
