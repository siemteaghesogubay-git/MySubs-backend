using MySubs.Repositories.interfaces;
using MySubs.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySubs.Data;


namespace MySubs.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository

    {
        private readonly ApplicationDbContext _context;

        public SubscriptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Subscription>> GetAllSubscriptionsForUserAsync(string userId)
        {
            return await _context.Subscriptions
                .Include(s => s.Category)
                .Where(s => s.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Subscription?> GetSubscriptionByIdAsync(int subscriptionId, string userId)
        {
            // UserId-filtret körs redan här i repository-lagret —
            // ingen annan användares data kan någonsin läcka ut från denna metod
            return await _context.Subscriptions
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.Id == subscriptionId && s.UserId == userId);
        }

        public async Task<Subscription> CreateSubscriptionAsync(Subscription newSubscription)
        {
            _context.Subscriptions.Add(newSubscription);
            await _context.SaveChangesAsync();
            return newSubscription;
        }

        public async Task<bool> UpdateSubscriptionAsync(Subscription subscription)
        {
            _context.Subscriptions.Update(subscription);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteSubscriptionAsync(int subscriptionId, string userId)
        {
            return await _context.Subscriptions
                .Where(s => s.Id == subscriptionId && s.UserId == userId)
                .ExecuteDeleteAsync() > 0;
        }
    }

}
