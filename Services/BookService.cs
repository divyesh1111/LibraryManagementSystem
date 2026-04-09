using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services
{
    public class BookService : IBookService
    {
        private readonly LibraryDbContext _context;

        public BookService(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.LibraryBranch)
                .OrderBy(b => b.Title)
                .ToListAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.LibraryBranch)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm)
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Where(b => b.Title.Contains(searchTerm) ||
                            b.ISBN.Contains(searchTerm) ||
                            (b.Author != null &&
                             (b.Author.FirstName.Contains(searchTerm) ||
                              b.Author.LastName.Contains(searchTerm))))
                .OrderBy(b => b.Title)
                .ToListAsync();
        }

        public async Task<Book> CreateBookAsync(Book book)
        {
            book.CreatedDate = DateTime.UtcNow;
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<Book?> UpdateBookAsync(int id, Book updatedBook)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return null;

            book.Title = updatedBook.Title;
            book.ISBN = updatedBook.ISBN;
            book.Description = updatedBook.Description;
            book.PublicationDate = updatedBook.PublicationDate;
            book.Publisher = updatedBook.Publisher;
            book.PageCount = updatedBook.PageCount;
            book.Language = updatedBook.Language;
            book.CoverImageUrl = updatedBook.CoverImageUrl;
            book.Price = updatedBook.Price;
            book.AvailableCopies = updatedBook.AvailableCopies;
            book.TotalCopies = updatedBook.TotalCopies;
            book.AuthorId = updatedBook.AuthorId;
            book.CategoryId = updatedBook.CategoryId;
            book.LibraryBranchId = updatedBook.LibraryBranchId;
            book.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;

            // Check for active loans
            var hasActiveLoans = await _context.BookLoans
                .AnyAsync(l => l.BookId == id && l.Status == LoanStatus.Active);

            if (hasActiveLoans) return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsbnExistsAsync(string isbn, int? excludeId = null)
        {
            if (excludeId.HasValue)
                return await _context.Books.AnyAsync(b => b.ISBN == isbn && b.Id != excludeId.Value);

            return await _context.Books.AnyAsync(b => b.ISBN == isbn);
        }
    }
}