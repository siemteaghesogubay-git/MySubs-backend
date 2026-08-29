using System.ComponentModel.DataAnnotations;

namespace MySubs.Dtos
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password", ErrorMessage = "Lösenorden matchar inte.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required] public string LastName { get;  set; } = string.Empty;

        [Required]
        [Phone]
        [MinLength(10)]
        [MaxLength(13)]
        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Ogiltigt telefonnummer.")]
        public string PhoneNumber { get; set; } = string.Empty;



    }

    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}