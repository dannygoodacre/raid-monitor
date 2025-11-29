using Microsoft.AspNetCore.Identity;
using RaidMonitor.Data.Repositories;

namespace RaidMonitor.Data.Tests.Repositories;

[TestFixture]
public class UserRepositoryTests : TestBase
{
    private ApplicationContext _context;

    private UserRepository _repository;

    private CancellationToken _cancellationToken;

    [SetUp]
    public void SetUp()
    {
        var factory = new ApplicationContextFactory();

        _context = factory.CreateInMemoryDbContext();

        _repository = new UserRepository(_context);

        _cancellationToken = CancellationToken.None;
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<IdentityUser<int>>
        {
            new()
            {
                Id = 123,
                Email = "user1@email.com"
            },
            new()
            {
                Id = 456,
                Email = "user2@email.com"
            }
        };

        _context.Users.AddRange(users);

        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync(_cancellationToken);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Count, Is.EqualTo(2));

            Assert.That(result[0].Id, Is.EqualTo(users[0].Id));
            Assert.That(result[0].Email, Is.EqualTo(users[0].Email));

            Assert.That(result[1].Id, Is.EqualTo(users[1].Id));
            Assert.That(result[1].Email, Is.EqualTo(users[1].Email));
        }
    }
}
