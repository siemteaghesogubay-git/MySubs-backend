using MySubs.Dtos;

namespace MySubs.Services.IServices
{
    public interface ICategoryService
    {
        Task<List<CategoryResponseDto>> GetAllCategoriesAsync();
        Task<CategoryResponseDto?> GetCategoryByIdAsync(int id);
        Task<CategoryResponseDto> CreateCategoryAsync(CategoryCreateDto dto);
        Task<bool> UpdateCategoryAsync(int id, CategoryCreateDto dto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
