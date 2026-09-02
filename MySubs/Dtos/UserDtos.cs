using System.ComponentModel.DataAnnotations;

namespace MySubs.Dtos
{
    public class UserResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }

    // prfile dtos

    public class UpdateProfileDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [MinLength(10)]
        [MaxLength(13)]
        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Ogiltigt telefonnummer.")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
