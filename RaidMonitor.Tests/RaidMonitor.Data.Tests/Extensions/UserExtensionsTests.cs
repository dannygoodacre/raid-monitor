using Microsoft.AspNetCore.Identity;
using RaidMonitor.Data.Extensions;

namespace RaidMonitor.Data.Tests.Extensions;

[TestFixture]
public class UserExtensionsTests : TestBase
{
    [Test]
    public void ToUser_MapsIdentityUserToUser()
    {
        // Arrange
        var identityUser = new IdentityUser<int>
        {
            Id = 123,
            Email = "test@email.com",
        };

        // Act
        var result = identityUser.ToModel();

        // Assert
        Assert.That(result.Email, Is.EqualTo(identityUser.Email));
    }
}
