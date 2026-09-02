using MySubs.Models;
using System.ComponentModel.DataAnnotations;

namespace MySubs.Dtos
{
    public class FamilyGroupCreateDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
    }

    public class AddMemberDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public FamilyRole Role { get; set; }
    }

    public class FamilyMemberResponseDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public FamilyRole Role { get; set; }
        public DateTime JoinedAt { get; set; }
    }

    public class FamilyGroupResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CreatedByUserId { get; set; } = string.Empty;
        public List<FamilyMemberResponseDto> Members { get; set; } = new();
    }
}

