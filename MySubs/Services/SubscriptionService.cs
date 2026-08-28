using MySubs.Data.Repositories.interfaces;
using MySubs.Dtos;
using MySubs.Models;
using MySubs.Services.IServices;

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
    }
}
