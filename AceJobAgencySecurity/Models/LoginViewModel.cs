using System.ComponentModel.DataAnnotations;

namespace AceJobAgencySecurity.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        // Optional for remembering the user on the same device
        public bool RememberMe { get; set; }

        // Added to hold the reCAPTCHA token
        [Required]
        public string RecaptchaResponse { get; set; } // This will hold the reCAPTCHA token
    }
}

