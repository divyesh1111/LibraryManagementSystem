using LibraryManagementSystem.Api.Dtos;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly LibraryDbContext _db;
        public CategoryService(LibraryDbContext db) => _db = db;

        public async Task<PagedResult<Category>> GetPagedAsync(string? searchTerm, int page, int pageSize)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var q = _db.Categories.AsNoTracking().Include(c => c.Books).AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchTerm))
                q = q.Where(c => c.Name.Contains(searchTerm) || (c.Description != null && c.Description.Contains(searchTerm)));

            q = q.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name);

            var total = await q.CountAsync();
            var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<Category> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
        }

        public Task<Category?> GetByIdAsync(int id) =>
            _db.Categories.AsNoTracking().Include(c => c.Books).FirstOrDefaultAsync(c => c.Id == id);

        public async Task<Category> CreateAsync(Category category)
        {
            category.CreatedDate = DateTime.UtcNow;
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
            return category;
        }

        public async Task<bool> UpdateAsync(int id, Category category)
        {
            var existing = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (existing == null) return false;

            existing.Name = category.Name;
            existing.Description = category.Description;
            existing.IconClass = category.IconClass;
            existing.ColorCode = category.ColorCode;
            existing.DisplayOrder = category.DisplayOrder;
            existing.IsActive = category.IsActive;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<(bool ok, string? reason)> DeleteAsync(int id)
        {
            var category = await _db.Categories.Include(c => c.Books).FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return (false, "Not found.");
            if (category.Books.Any()) return (false, "Cannot delete category with associated books.");

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();
            return (true, null);
        }
    }
}