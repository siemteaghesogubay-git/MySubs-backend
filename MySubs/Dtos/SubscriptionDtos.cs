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
        public string CategoryName { get; set; } = string.Empty; 
    }

    public class CategorySummaryDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal TotalMonthlyCost { get; set; }
        public int SubscriptionCount { get; set; }
    }

    public class DashboardSummaryDto
    {
        public decimal TotalMonthlyCost { get; set; }
        public int ActiveSubscriptionCount { get; set; }
        public int CancelledSubscriptionCount { get; set; }
        public List<CategorySummaryDto> CostByCategory { get; set; } = new();
    }

    public class UpcomingPaymentDto
    {
        public int SubscriptionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public DateTime NextPaymentDate { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
