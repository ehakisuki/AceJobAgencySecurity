using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System;

namespace AceJobAgencySecurity.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string? FirstName { get; set; } = "";

        [Required]
        public string? LastName { get; set; } = "";

        [Required]
        public string? Gender { get; set; } = "";

        [Required]
        public string? NRIC { get; set; } = "";  // NRIC will be stored as-is without encryption

        [Required]
        [EmailAddress]
        public string? Email { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        [MinLength(12, ErrorMessage = "Password must be at least 12 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{12,}$",
            ErrorMessage = "Password must include uppercase, lowercase, numbers, and special characters.")]
        public string? Password { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; } = "";

        [Required]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        public string? WhoAmI { get; set; } = ""; // Allow all special characters

        public IFormFile? Resume { get; set; }

        // Remove NRIC encryption method as it is no longer needed
    }
}











