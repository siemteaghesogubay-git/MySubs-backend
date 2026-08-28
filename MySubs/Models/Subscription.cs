using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MySubs.Models
{
    public enum BillingInterval { Monthly, Yearly }

    public class Subscription
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 100000)]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Cost { get; set; }

        [Required]
        public BillingInterval Interval { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? CancelledDate { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        // Sätts av servern utifrån inloggad användare - ALDRIG från klienten
        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
    }
}
