using System.ComponentModel.DataAnnotations;

namespace TaskManagementApi.DTOs
{
    public class RegisterUserDto
    {
        // [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        // [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        [Required]
        // [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }

        // [Required(ErrorMessage = "Password is required.")]
        // [DataType(DataType.Password)]
        // [StringLength(100, MinimumLength = 8, ErrorMessage = "The password must be between {2} and {100} characters long.")]
        // [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
        //     ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; } = String.Empty;

        // [Required(ErrorMessage = "Confirm password is required.")]
        // [DataType(DataType.Password)]
        // [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = String.Empty;
    }
}