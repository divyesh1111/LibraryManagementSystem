using LibraryManagementSystem.Api.Dtos;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services
{
    public interface ILibraryBranchService
    {
        Task<PagedResult<LibraryBranch>> GetPagedAsync(string? searchTerm, bool? activeOnly, string? sortBy, int page, int pageSize);
        Task<LibraryBranch?> GetByIdAsync(int id);
        Task<LibraryBranch> CreateAsync(LibraryBranch branch);
        Task<bool> UpdateAsync(int id, LibraryBranch branch);
        Task<(bool ok, string? reason)> DeleteAsync(int id);
    }
}