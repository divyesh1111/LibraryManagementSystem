using LibraryManagementSystem.Api.Dtos;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Exceptions;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services
{
    public class BookService : IBookService
    {
        private readonly LibraryDbContext _db;
        public BookService(LibraryDbContext db) => _db = db;

        public async Task<PagedResult<Book>> GetPagedAsync(string? searchTerm, int? categoryId, int? authorId, int? branchId, bool? availableOnly, string? sortBy, int page, int pageSize)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var q = _db.Books.AsNoTracking()
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.LibraryBranch)
                .Include(b => b.Reviews)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                q = q.Where(b => b.Title.Contains(searchTerm) ||
                                 b.ISBN.Contains(searchTerm) ||
                                 (b.Author != null && (b.Author.FirstName.Contains(searchTerm) || b.Author.LastName.Contains(searchTerm))));
            }

            if (categoryId.HasValue) q = q.Where(b => b.CategoryId == categoryId);
            if (authorId.HasValue) q = q.Where(b => b.AuthorId == authorId);
            if (branchId.HasValue) q = q.Where(b => b.LibraryBranchId == branchId);
            if (availableOnly == true) q = q.Where(b => b.AvailableCopies > 0);

            q = sortBy switch
            {
                "title_desc" => q.OrderByDescending(b => b.Title),
                "author" => q.OrderBy(b => b.Author!.LastName),
                "newest" => q.OrderByDescending(b => b.PublicationDate),
                "oldest" => q.OrderBy(b => b.PublicationDate),
                _ => q.OrderBy(b => b.Title)
            };

            var total = await q.CountAsync();
            var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<Book> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
        }

        public Task<Book?> GetByIdAsync(int id) =>
            _db.Books.AsNoTracking()
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.LibraryBranch)
                .Include(b => b.Reviews).ThenInclude(r => r.Customer)
                .FirstOrDefaultAsync(b => b.Id == id);

        public async Task<Book> CreateAsync(Book book)
        {
            if (await _db.Books.AnyAsync(b => b.ISBN == book.ISBN))
                throw new DuplicateIsbnException(book.ISBN);

            book.CreatedDate = DateTime.UtcNow;
            _db.Books.Add(book);
            await _db.SaveChangesAsync();
            return book;
        }

        public async Task<bool> UpdateAsync(int id, Book book)
        {
            var existing = await _db.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (existing == null) return false;

            if (await _db.Books.AnyAsync(b => b.ISBN == book.ISBN && b.Id != id))
                throw new DuplicateIsbnException(book.ISBN);

            existing.Title = book.Title;
            existing.ISBN = book.ISBN;
            existing.Description = book.Description;
            existing.PublicationDate = book.PublicationDate;
            existing.Publisher = book.Publisher;
            existing.PageCount = book.PageCount;
            existing.Language = book.Language;
            existing.CoverImageUrl = book.CoverImageUrl;
            existing.Price = book.Price;
            existing.AvailableCopies = book.AvailableCopies;
            existing.TotalCopies = book.TotalCopies;
            existing.AuthorId = book.AuthorId;
            existing.CategoryId = book.CategoryId;
            existing.LibraryBranchId = book.LibraryBranchId;
            existing.UpdatedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<(bool ok, string? reason)> DeleteAsync(int id)
        {
            var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (book == null) return (false, "Not found.");

            var hasActiveLoans = await _db.BookLoans.AnyAsync(l => l.BookId == id && l.Status == LoanStatus.Active);
            if (hasActiveLoans) return (false, "Cannot delete book with active loans.");

            _db.Books.Remove(book);
            await _db.SaveChangesAsync();
            return (true, null);
        }
    }
}