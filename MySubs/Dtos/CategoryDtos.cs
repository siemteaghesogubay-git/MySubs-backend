using System.ComponentModel.DataAnnotations;

namespace MySubs.Dtos
{
    public class CategoryCreateDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [StringLength(7)]
        [RegularExpression("^#([A-Fa-f0-9]{6})$", ErrorMessage = "Färg måste vara en giltig hex-kod.")]
        public string? Color { get; set; }
    }

    public class CategoryResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Color { get; set; }
    }
}
