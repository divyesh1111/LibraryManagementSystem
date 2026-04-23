using LibraryManagementSystem.Api.Dtos;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services
{
    public interface IBookService
    {
        Task<PagedResult<Book>> GetPagedAsync(string? searchTerm, int? categoryId, int? authorId, int? branchId, bool? availableOnly, string? sortBy, int page, int pageSize);
        Task<Book?> GetByIdAsync(int id);
        Task<Book> CreateAsync(Book book);
        Task<bool> UpdateAsync(int id, Book book);
        Task<(bool ok, string? reason)> DeleteAsync(int id);
    }
}