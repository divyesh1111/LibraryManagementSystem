using LibraryManagementSystem.Api.Dtos;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services
{
    public interface IAuthorService
    {
        Task<PagedResult<Author>> GetPagedAsync(string? searchTerm, string? sortBy, int page, int pageSize);
        Task<Author?> GetByIdAsync(int id);
        Task<Author> CreateAsync(Author author);
        Task<bool> UpdateAsync(int id, Author author);
        Task<(bool ok, string? reason)> DeleteAsync(int id);
    }
}