using MySubs.Data.Repositories.interfaces;
using MySubs.Dtos;
using MySubs.Models;
using MySubs.Services.IServices;

namespace MySubs.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<CategoryResponseDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return categories.Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Color = c.Color
            }).ToList();
        }

        public async Task<CategoryResponseDto?> GetCategoryByIdAsync(int id)
        {
            var c = await _categoryRepository.GetCategoryByIdAsync(id);
            return c is null ? null : new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Color = c.Color
            };
        }

        public async Task<CategoryResponseDto> CreateCategoryAsync(CategoryCreateDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                Color = dto.Color
            };

            var created = await _categoryRepository.CreateCategoryAsync(category);
            return new CategoryResponseDto
            {
                Id = created.Id,
                Name = created.Name,
                Color = created.Color
            };
        }

        public async Task<bool> UpdateCategoryAsync(int id, CategoryUpdateDto dto)
        {
            var existing = await _categoryRepository.GetCategoryByIdAsync(id);
            if (existing is null) return false;

            existing.Name = dto.Name;
            existing.Color = dto.Color;

            return await _categoryRepository.UpdateCategoryAsync(existing);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            return await _categoryRepository.DeleteCategoryAsync(id);
        }
    }
}