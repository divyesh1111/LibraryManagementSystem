using LibraryManagementSystem.Api.Dtos;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services
{
    public interface ICustomerService
    {
        Task<PagedResult<Customer>> GetPagedAsync(string? searchTerm, bool? activeOnly, string? sortBy, int page, int pageSize);
        Task<Customer?> GetByIdAsync(int id);
        Task<Customer> CreateAsync(Customer customer);
        Task<bool> UpdateAsync(int id, Customer customer);
        Task<(bool ok, string? reason)> DeleteAsync(int id);
    }
}