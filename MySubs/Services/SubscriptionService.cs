using MySubs.Repositories.interfaces;
using MySubs.Dtos;
using MySubs.Models;
using MySubs.Services.IServices;
using MySubs.Data;
using Microsoft.EntityFrameworkCore;
using MySubs.Repositories;


namespace MySubs.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ICategoryRepository _categoryRepository;

        public SubscriptionService(
            ISubscriptionRepository subscriptionRepository,
            ICategoryRepository categoryRepository)
        {
            _subscriptionRepository = subscriptionRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<List<SubscriptionResponseDto>> GetAllSubscriptionsAsync(string userId)
        {
            var subscriptions = await _subscriptionRepository.GetAllSubscriptionsForUserAsync(userId);
            return subscriptions.Select(MapToResponseDto).ToList();
        }

        public async Task<SubscriptionResponseDto?> GetSubscriptionByIdAsync(int id, string userId)
        {
            var subscription = await _subscriptionRepository.GetSubscriptionByIdAsync(id, userId);
            return subscription is null ? null : MapToResponseDto(subscription);
        }

        public async Task<SubscriptionResponseDto> CreateSubscriptionAsync(SubscriptionCreateDto dto, string userId)
        {
            // Affärslogik: validera att kategorin faktiskt finns innan vi skapar
            var category = await _categoryRepository.GetCategoryByIdAsync(dto.CategoryId)
                ?? throw new ArgumentException("Kategorin finns inte.");

            var subscription = new Subscription
            {
                Name = dto.Name,
                Cost = dto.Cost,
                Interval = dto.Interval,
                StartDate = dto.StartDate,
                CategoryId = dto.CategoryId,
                UserId = userId 
            };

            var created = await _subscriptionRepository.CreateSubscriptionAsync(subscription);
            created.Category = category; 
            return MapToResponseDto(created);
        }

        public async Task<bool> UpdateSubscriptionAsync(int id, SubscriptionUpdateDto dto, string userId)
        {
            // Hämtas via repository som redan filtrerar på userId —
            // om den inte hittas ägs den antingen inte av användaren eller finns inte alls
            var existing = await _subscriptionRepository.GetSubscriptionByIdAsync(id, userId);
            if (existing is null) return false;

            existing.Name = dto.Name;
            existing.Cost = dto.Cost;
            existing.Interval = dto.Interval;
            existing.StartDate = dto.StartDate;
            existing.CancelledDate = dto.CancelledDate;
            existing.CategoryId = dto.CategoryId;

            return await _subscriptionRepository.UpdateSubscriptionAsync(existing);
        }

        public async Task<bool> DeleteSubscriptionAsync(int id, string userId)
        {
            return await _subscriptionRepository.DeleteSubscriptionAsync(id, userId);
        }

        private static SubscriptionResponseDto MapToResponseDto(Subscription s)
        {
            return new SubscriptionResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                Cost = s.Cost,
                Interval = s.Interval,
                StartDate = s.StartDate,
                CancelledDate = s.CancelledDate,
                CategoryId = s.CategoryId,
                CategoryName = s.Category?.Name ?? string.Empty
            };
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(string userId)
        {
            var subscriptions = await _subscriptionRepository.GetAllSubscriptionsForUserAsync(userId);

            var active = subscriptions.Where(s => s.CancelledDate is null).ToList();
            var cancelled = subscriptions.Where(s => s.CancelledDate is not null).ToList();

            // Räkna om årliga kostnader till per månad så allt är jämförbart
            decimal MonthlyEquivalent(Subscription s) =>
                s.Interval == BillingInterval.Yearly ? s.Cost / 12 : s.Cost;

            var totalMonthlyCost = active.Sum(MonthlyEquivalent);

            var costByCategory = active
                .GroupBy(s => s.Category.Name)
                .Select(g => new CategorySummaryDto
                {
                    CategoryName = g.Key,
                    TotalMonthlyCost = g.Sum(MonthlyEquivalent),
                    SubscriptionCount = g.Count()
                })
                .OrderByDescending(c => c.TotalMonthlyCost)
                .ToList();

            return new DashboardSummaryDto
            {
                TotalMonthlyCost = totalMonthlyCost,
                ActiveSubscriptionCount = active.Count,
                CancelledSubscriptionCount = cancelled.Count,
                CostByCategory = costByCategory
            };
        }







        private static DateTime GetNextPaymentDate(Subscription subscription)
        {
            var start = subscription.StartDate;
            var now = DateTime.UtcNow;

            if (subscription.Interval == BillingInterval.Monthly)
            {
                var next = start;
                while (next < now)
                {
                    next = next.AddMonths(1);
                }
                return next;
            }
            else // Yearly
            {
                var next = start;
                while (next < now)
                {
                    next = next.AddYears(1);
                }
                return next;
            }
        }



            public async Task<List<UpcomingPaymentDto>> GetUpcomingPaymentsAsync(string userId, int daysAhead = 7)
        {
            var subscriptions = await _subscriptionRepository.GetAllSubscriptionsForUserAsync(userId);
            var active = subscriptions.Where(s => s.CancelledDate is null);

            var cutoff = DateTime.UtcNow.AddDays(daysAhead);

            var upcoming = active
                .Select(s => new
                {
                    Subscription = s,
                    NextPayment = GetNextPaymentDate(s)
                })
                .Where(x => x.NextPayment <= cutoff)
                .OrderBy(x => x.NextPayment)
                .Select(x => new UpcomingPaymentDto
                {
                    SubscriptionId = x.Subscription.Id,
                    Name = x.Subscription.Name,
                    Cost = x.Subscription.Cost,
                    NextPaymentDate = x.NextPayment,
                    CategoryName = x.Subscription.Category.Name
                })
                .ToList();

            return upcoming;
        }
    }
    }           
    

