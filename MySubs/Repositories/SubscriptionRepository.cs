using MySubs.Repositories.interfaces;
using MySubs.Models;
using MySubs.Dtos;
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

        public async Task<List<SubscriptionResponseDto>> GetAllSubscriptionsAsync(string userId, SubscriptionFilterDto? filter = null)
        {
            var subscriptions = await _context.Subscriptions
                .Include(s => s.Category)
                .Where(s => s.UserId == userId)
                .AsNoTracking()
                .ToListAsync();

            var query = subscriptions.AsEnumerable();

            if (filter is not null)
            {
                if (filter.CategoryId.HasValue)
                    query = query.Where(s => s.CategoryId == filter.CategoryId.Value);

                if (filter.ActiveOnly == true)
                    query = query.Where(s => s.CancelledDate is null);

                query = filter.SortBy?.ToLower() switch
                {
                    "cost" => filter.SortDescending == true
                        ? query.OrderByDescending(s => s.Cost)
                        : query.OrderBy(s => s.Cost),
                    "startdate" => filter.SortDescending == true
                        ? query.OrderByDescending(s => s.StartDate)
                        : query.OrderBy(s => s.StartDate),
                    "name" => filter.SortDescending == true
                        ? query.OrderByDescending(s => s.Name)
                        : query.OrderBy(s => s.Name),
                    _ => query
                };
            }

            return query.Select(s => new SubscriptionResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                Cost = s.Cost,
                Interval = s.Interval,
                StartDate = s.StartDate,
                CancelledDate = s.CancelledDate,
                CategoryId = s.CategoryId,
                CategoryName = s.Category?.Name ?? string.Empty
            }).ToList();
        }
    }

}
