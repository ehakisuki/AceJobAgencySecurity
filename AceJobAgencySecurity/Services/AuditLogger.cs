using AceJobAgencySecurity.Data;
using AceJobAgencySecurity.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AceJobAgencySecurity.Services
{
    public class AuditLogger : IAuditLogger
    {
        private readonly ApplicationDbContext _context;

        public AuditLogger(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogActivityAsync(string userId, string activity)
        {
            var userActivity = new UserActivity
            {
                UserId = userId,
                Activity = activity,
                Timestamp = DateTime.UtcNow
            };

            // Save the activity in the database
            await _context.UserActivities.AddAsync(userActivity);
            await _context.SaveChangesAsync();
        }
    }
}
