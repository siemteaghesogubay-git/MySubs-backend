using MySubs.Models;
using System.ComponentModel.DataAnnotations;

namespace MySubs.Dtos
{
    public class SubscriptionCreateDto
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 100000, ErrorMessage = "Kostnad måste vara större än 0.")]
        public decimal Cost { get; set; }

        [Required]
        public BillingInterval Interval { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }

    public class SubscriptionUpdateDto : SubscriptionCreateDto
    {
        public DateTime? CancelledDate { get; set; }
    }

    public class SubscriptionResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public BillingInterval Interval { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? CancelledDate { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty; // slår ihop lite data, slipper extra anrop i frontend
    }
}
