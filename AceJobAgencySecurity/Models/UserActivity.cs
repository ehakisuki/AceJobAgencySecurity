using System;

namespace AceJobAgencySecurity.Models
{
    public class UserActivity
    {
        public int Id { get; set; }             // Primary key
        public string UserId { get; set; }      // Foreign key to User (Identity)
        public string Activity { get; set; }    // Description of the activity (e.g., "User logged in")
        public DateTime Timestamp { get; set; } // When the activity took place
    }
}
