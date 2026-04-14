using LibraryManagementSystem.Api.Dtos;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services
{
    public interface IBookLoanService
    {
        Task<PagedResult<BookLoan>> GetPagedAsync(string? searchTerm, LoanStatus? status, int page, int pageSize);
        Task<BookLoan?> GetByIdAsync(int id);
        Task<BookLoan> CreateAsync(BookLoan loan);
        Task<bool> UpdateAsync(int id, BookLoan loan);
        Task<(bool ok, string? reason)> DeleteAsync(int id);

        Task<int> MarkOverdueAsync();
        Task<(bool ok, string? reason)> ReturnAsync(int loanId, DateTime returnDateUtc, string? notes = null);
    }
}