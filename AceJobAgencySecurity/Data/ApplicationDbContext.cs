using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AceJobAgencySecurity.Models;

namespace AceJobAgencySecurity.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<UserActivity> UserActivities { get; set; }  // Add this DbSet to track user activities
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Optionally, you can add DbSet for other models if needed
        // public DbSet<OtherModel> OtherModels { get; set; }
    }
}

