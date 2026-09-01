using MySubs.Dtos;
using MySubs.Models;

namespace MySubs.Repositories.interfaces
{
    public interface ISubscriptionRepository
    {
        Task<List<Subscription>> GetAllSubscriptionsForUserAsync(string userId);
        Task<Subscription?> GetSubscriptionByIdAsync(int subscriptionId, string userId);
        Task<Subscription> CreateSubscriptionAsync(Subscription newSubscription);
        Task<bool> UpdateSubscriptionAsync(Subscription subscription);
        Task<bool> DeleteSubscriptionAsync(int subscriptionId, string userId);
       
    }
}
