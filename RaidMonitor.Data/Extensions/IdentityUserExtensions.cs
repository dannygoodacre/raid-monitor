using Microsoft.AspNetCore.Identity;
using RaidMonitor.Core.Entities;

namespace RaidMonitor.Data.Extensions;

public static class IdentityUserExtensions
{
    public static User ToUser(this IdentityUser user) =>
        new()
        {
            Id = user.Id,
            Email = user.Email!,
        };
}
