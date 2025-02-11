namespace AceJobAgencySecurity.Services
{
    public interface IRateLimitingService
    {
        Task<bool> IsLockedOutAsync(string email);
        Task IncrementFailedAttemptsAsync(string email);
        Task ResetFailedAttemptsAsync(string email);
    }
}

