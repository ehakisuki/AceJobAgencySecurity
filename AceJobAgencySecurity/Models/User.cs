using Microsoft.AspNetCore.Identity;
using System;

namespace AceJobAgencySecurity.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;

        // NRIC is now a simple string without encryption and is not required
        public string NRIC { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }
        public string WhoAmI { get; set; } = string.Empty;

        // Track the last password change date
        public DateTime? LastPasswordChanged { get; set; }

        // Default constructor for Entity Framework
        public User() { }

        // Constructor without encryption for NRIC
        public User(string firstName, string lastName, string gender, string nric, DateTime dateOfBirth, string whoAmI)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Gender = gender ?? throw new ArgumentNullException(nameof(gender));
            NRIC = nric ?? throw new ArgumentNullException(nameof(nric)); // No encryption here
            DateOfBirth = dateOfBirth;
            WhoAmI = whoAmI ?? string.Empty;
        }
    }
}






