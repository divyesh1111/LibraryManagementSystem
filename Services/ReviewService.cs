using LibraryManagementSystem.Api.Dtos;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services
{
    public class ReviewService : IReviewService
    {
        private readonly LibraryDbContext _db;
        public ReviewService(LibraryDbContext db) => _db = db;

        public async Task<PagedResult<Review>> GetPagedAsync(int? bookId, bool includeUnapproved, int page, int pageSize)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var q = _db.Reviews.AsNoTracking()
                .Include(r => r.Book)
                .Include(r => r.Customer)
                .AsQueryable();

            if (bookId.HasValue) q = q.Where(r => r.BookId == bookId.Value);
            if (!includeUnapproved) q = q.Where(r => r.IsApproved);

            q = q.OrderByDescending(r => r.CreatedDate);

            var total = await q.CountAsync();
            var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<Review> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
        }

        public Task<Review?> GetByIdAsync(int id) =>
            _db.Reviews.AsNoTracking().Include(r => r.Book).Include(r => r.Customer).FirstOrDefaultAsync(r => r.Id == id);

        public async Task<(bool ok, string? reason, Review? review)> CreateAsync(Review review, string currentUserEmail)
        {
            // Security: ensure CustomerId belongs to current user email
            var customer = await _db.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == review.CustomerId);
            if (customer == null) return (false, "Customer not found.", null);

            if (!string.Equals(customer.Email, currentUserEmail, StringComparison.OrdinalIgnoreCase))
                return (false, "You can only create reviews for your own customer record (email mismatch).", null);

            // Must have borrowed the book
            var borrowed = await _db.BookLoans.AsNoTracking()
                .AnyAsync(l => l.CustomerId == review.CustomerId && l.BookId == review.BookId);
            if (!borrowed) return (false, "You must borrow the book before reviewing it.", null);

            // One review per (BookId, CustomerId)
            var exists = await _db.Reviews.AnyAsync(r => r.BookId == review.BookId && r.CustomerId == review.CustomerId);
            if (exists) return (false, "You already reviewed this book.", null);

            review.IsApproved = false;
            review.CreatedDate = DateTime.UtcNow;

            _db.Reviews.Add(review);
            await _db.SaveChangesAsync();
            return (true, null, review);
        }

        public async Task<bool> UpdateAsync(int id, Review review)
        {
            var existing = await _db.Reviews.FirstOrDefaultAsync(r => r.Id == id);
            if (existing == null) return false;

            existing.Rating = review.Rating;
            existing.Title = review.Title;
            existing.Content = review.Content;
            existing.UpdatedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<(bool ok, string? reason)> DeleteAsync(int id)
        {
            var existing = await _db.Reviews.FirstOrDefaultAsync(r => r.Id == id);
            if (existing == null) return (false, "Not found.");

            _db.Reviews.Remove(existing);
            await _db.SaveChangesAsync();
            return (true, null);
        }

        public async Task<bool> SetApprovalAsync(int id, bool approved)
        {
            var r = await _db.Reviews.FirstOrDefaultAsync(x => x.Id == id);
            if (r == null) return false;
            r.IsApproved = approved;
            r.UpdatedDate = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IncrementHelpfulAsync(int id)
        {
            var r = await _db.Reviews.FirstOrDefaultAsync(x => x.Id == id);
            if (r == null) return false;
            r.HelpfulVotes++;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}