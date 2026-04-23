using LibraryManagementSystem.Api.Dtos;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Exceptions;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services
{
    public class BookLoanService : IBookLoanService
    {
        private readonly LibraryDbContext _db;
        public BookLoanService(LibraryDbContext db) => _db = db;

        public async Task<PagedResult<BookLoan>> GetPagedAsync(string? searchTerm, LoanStatus? status, int page, int pageSize)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var q = _db.BookLoans.AsNoTracking()
                .Include(l => l.Book)
                .Include(l => l.Customer)
                .Include(l => l.LibraryBranch)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                q = q.Where(l => (l.Book != null && l.Book.Title.Contains(searchTerm)) ||
                                 (l.Customer != null && (l.Customer.FirstName.Contains(searchTerm) || l.Customer.LastName.Contains(searchTerm))));
            }

            if (status.HasValue) q = q.Where(l => l.Status == status);

            q = q.OrderByDescending(l => l.LoanDate);

            var total = await q.CountAsync();
            var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<BookLoan> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
        }

        public Task<BookLoan?> GetByIdAsync(int id) =>
            _db.BookLoans.AsNoTracking()
                .Include(l => l.Book)
                .Include(l => l.Customer)
                .Include(l => l.LibraryBranch)
                .FirstOrDefaultAsync(l => l.Id == id);

        public async Task<BookLoan> CreateAsync(BookLoan loan)
        {
            var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == loan.BookId);
            if (book == null) throw new BookNotFoundException(loan.BookId);
            if (book.AvailableCopies <= 0) throw new UnauthorizedLoanException(loan.CustomerId, "Book is not available.");

            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Id == loan.CustomerId);
            if (customer == null) throw new CustomerNotFoundException(loan.CustomerId);

            var hasOverdue = await _db.BookLoans.AnyAsync(l => l.CustomerId == loan.CustomerId && l.Status == LoanStatus.Overdue);
            if (hasOverdue) throw new UnauthorizedLoanException(loan.CustomerId, "Customer has overdue books.");

            loan.Status = LoanStatus.Active;
            loan.CreatedDate = DateTime.UtcNow;

            book.AvailableCopies--;

            _db.BookLoans.Add(loan);
            await _db.SaveChangesAsync();
            return loan;
        }

        public async Task<bool> UpdateAsync(int id, BookLoan loan)
        {
            var existing = await _db.BookLoans.FirstOrDefaultAsync(l => l.Id == id);
            if (existing == null) return false;

            existing.DueDate = loan.DueDate;
            existing.ReturnDate = loan.ReturnDate;
            existing.Status = loan.Status;
            existing.FineAmount = loan.FineAmount;
            existing.IsFinePaid = loan.IsFinePaid;
            existing.Notes = loan.Notes;
            existing.UpdatedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<(bool ok, string? reason)> DeleteAsync(int id)
        {
            var loan = await _db.BookLoans.Include(l => l.Book).FirstOrDefaultAsync(l => l.Id == id);
            if (loan == null) return (false, "Not found.");

            // If deleting an active loan, return the book to inventory
            if (loan.Status == LoanStatus.Active && loan.Book != null)
                loan.Book.AvailableCopies++;

            _db.BookLoans.Remove(loan);
            await _db.SaveChangesAsync();
            return (true, null);
        }

        public async Task<int> MarkOverdueAsync()
        {
            var now = DateTime.UtcNow;
            var overdue = await _db.BookLoans
                .Where(l => l.Status == LoanStatus.Active && l.DueDate < now)
                .ToListAsync();

            foreach (var l in overdue)
            {
                l.Status = LoanStatus.Overdue;
                var days = (now - l.DueDate).Days;
                l.FineAmount = Math.Max(0, days) * 0.50m;
            }

            await _db.SaveChangesAsync();
            return overdue.Count;
        }

        public async Task<(bool ok, string? reason)> ReturnAsync(int loanId, DateTime returnDateUtc, string? notes = null)
        {
            var loan = await _db.BookLoans.Include(l => l.Book).FirstOrDefaultAsync(l => l.Id == loanId);
            if (loan == null) return (false, "Not found.");

            if (loan.Status != LoanStatus.Active && loan.Status != LoanStatus.Overdue)
                return (false, "Loan is not returnable in current status.");

            loan.ReturnDate = returnDateUtc;
            loan.Status = LoanStatus.Returned;
            loan.Notes = notes;
            loan.UpdatedDate = DateTime.UtcNow;

            if (loan.Book != null)
                loan.Book.AvailableCopies++;

            // Fine calculation
            var daysOverdue = (returnDateUtc - loan.DueDate).Days;
            loan.FineAmount = daysOverdue > 0 ? daysOverdue * 0.50m : 0;

            await _db.SaveChangesAsync();
            return (true, null);
        }
    }
}