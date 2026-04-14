using LibraryManagementSystem.Api.Dtos;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services
{
    public interface ICategoryService
    {
        Task<PagedResult<Category>> GetPagedAsync(string? searchTerm, int page, int pageSize);
        Task<Category?> GetByIdAsync(int id);
        Task<Category> CreateAsync(Category category);
        Task<bool> UpdateAsync(int id, Category category);
        Task<(bool ok, string? reason)> DeleteAsync(int id);
    }
}