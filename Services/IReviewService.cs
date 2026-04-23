using LibraryManagementSystem.Api.Dtos;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services
{
    public interface IReviewService
    {
        Task<PagedResult<Review>> GetPagedAsync(int? bookId, bool includeUnapproved, int page, int pageSize);
        Task<Review?> GetByIdAsync(int id);
        Task<(bool ok, string? reason, Review? review)> CreateAsync(Review review, string currentUserEmail);
        Task<bool> UpdateAsync(int id, Review review);
        Task<(bool ok, string? reason)> DeleteAsync(int id);

        Task<bool> SetApprovalAsync(int id, bool approved);
        Task<bool> IncrementHelpfulAsync(int id);
    }
}