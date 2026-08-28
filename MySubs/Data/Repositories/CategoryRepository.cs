using MySubs.Data.Repositories.interfaces;
using MySubs.Models;
using Microsoft.EntityFrameworkCore;

namespace MySubs.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.AsNoTracking().ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
        }

        public async Task<Category> CreateCategoryAsync(Category newCategory)
        {
            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();
            return newCategory;
        }

        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            return await _context.Categories
                .Where(c => c.Id == categoryId)
                .ExecuteDeleteAsync() > 0;
        }
    }
}
