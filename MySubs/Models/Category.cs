using System.ComponentModel.DataAnnotations;

namespace MySubs.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [StringLength(7)]
        [RegularExpression("^#([A-Fa-f0-9]{6})$", ErrorMessage = "Färg måste vara en giltig hex-kod.")]
        public string? Color { get; set; }

        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>(); 
    }
}