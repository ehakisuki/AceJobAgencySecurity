namespace AceJobAgencySecurity.Services
{
    public interface IAuditLogger
    {
        Task LogActivityAsync(string userId, string activity);
    }
}
