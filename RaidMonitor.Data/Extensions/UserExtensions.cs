using Microsoft.AspNetCore.Identity;
using RaidMonitor.Core.Models;

namespace RaidMonitor.Data.Extensions;

internal static class UserExtensions
{
    public static User ToModel(this IdentityUser<int> value)
        => new()
        {
            Id = value.Id,
            Email = value.Email!,
        };
}
