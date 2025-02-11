using Microsoft.Extensions.Caching.Distributed;

namespace AceJobAgencySecurity.Services
{
    public class RateLimitingService : IRateLimitingService
    {
        private readonly IDistributedCache _cache;
        private const int MaxFailedAttempts = 3;
        private const int LockoutDurationInMinutes = 15;

        public RateLimitingService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<bool> IsLockedOutAsync(string email)
        {
            var failedAttempts = await _cache.GetStringAsync($"{email}_failedAttempts");
            if (failedAttempts != null && int.Parse(failedAttempts) >= MaxFailedAttempts)
            {
                var lockoutTime = await _cache.GetStringAsync($"{email}_lockoutTime");
                if (lockoutTime != null && DateTime.TryParse(lockoutTime, out var lockoutExpiration))
                {
                    if (lockoutExpiration > DateTime.Now)
                    {
                        return true; // Account is locked
                    }
                    await ResetFailedAttemptsAsync(email); // Reset after lockout period expires
                }
            }
            return false;
        }

        public async Task IncrementFailedAttemptsAsync(string email)
        {
            var failedAttempts = await _cache.GetStringAsync($"{email}_failedAttempts");
            int attempts = failedAttempts != null ? int.Parse(failedAttempts) : 0;

            if (attempts + 1 >= MaxFailedAttempts)
            {
                var lockoutTime = DateTime.Now.AddMinutes(LockoutDurationInMinutes).ToString();
                await _cache.SetStringAsync($"{email}_lockoutTime", lockoutTime, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(LockoutDurationInMinutes)
                });
            }

            await _cache.SetStringAsync($"{email}_failedAttempts", (attempts + 1).ToString(), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            });
        }

        public async Task ResetFailedAttemptsAsync(string email)
        {
            await _cache.RemoveAsync($"{email}_failedAttempts");
            await _cache.RemoveAsync($"{email}_lockoutTime");
        }
    }
}
