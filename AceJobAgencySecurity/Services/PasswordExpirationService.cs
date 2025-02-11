using AceJobAgencySecurity.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

public class PasswordExpirationService
{
    private readonly UserManager<User> _userManager;

    public PasswordExpirationService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    // Check if password has expired (based on your defined max password age, e.g., 90 days)
    public async Task<bool> HasPasswordExpiredAsync(User user)
    {
        // You should replace this with your actual expiration policy (e.g., 90 days)
        var maxPasswordAge = TimeSpan.FromDays(90);

        var lastPasswordChanged = user.LastPasswordChanged ?? DateTime.MinValue;

        return DateTime.UtcNow - lastPasswordChanged > maxPasswordAge;
    }
}

