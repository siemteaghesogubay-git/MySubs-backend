using MySubs.Dtos;

namespace MySubs.Services.IServices
{
    public interface ISubscriptionService
    {
        Task<List<SubscriptionResponseDto>> GetAllSubscriptionsAsync(string userId);
        Task<SubscriptionResponseDto?> GetSubscriptionByIdAsync(int id, string userId);
        Task<SubscriptionResponseDto> CreateSubscriptionAsync(SubscriptionCreateDto dto, string userId);
        Task<bool> UpdateSubscriptionAsync(int id, SubscriptionUpdateDto dto, string userId);
        Task<bool> DeleteSubscriptionAsync(int id, string userId);
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(string userId);
        Task<List<UpcomingPaymentDto>> GetUpcomingPaymentsAsync(string userId, int daysAhead);
    }
}
