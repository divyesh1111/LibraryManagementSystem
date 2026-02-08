using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class BookLoansController : Controller
    {
        private readonly LibraryDbContext _context;

        public BookLoansController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: BookLoans
        public async Task<IActionResult> Index(string? searchTerm, LoanStatus? statusFilter, int pageNumber = 1)
        {
            var query = _context.BookLoans
                .Include(l => l.Book)
                .Include(l => l.Customer)
                .Include(l => l.LibraryBranch)
                .AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(l => (l.Book != null && l.Book.Title.Contains(searchTerm)) ||
                                         (l.Customer != null && (l.Customer.FirstName.Contains(searchTerm) || 
                                          l.Customer.LastName.Contains(searchTerm))));
            }

            // Filter
            if (statusFilter.HasValue)
                query = query.Where(l => l.Status == statusFilter);

            // Order by loan date descending
            query = query.OrderByDescending(l => l.LoanDate);

            var pageSize = 10;
            var totalCount = await query.CountAsync();

            var loans = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new BookLoanViewModel
                {
                    Id = l.Id,
                    BookId = l.BookId,
                    BookTitle = l.Book != null ? l.Book.Title : "Unknown",
                    CustomerId = l.CustomerId,
                    CustomerName = l.Customer != null ? $"{l.Customer.FirstName} {l.Customer.LastName}" : "Unknown",
                    LibraryBranchId = l.LibraryBranchId,
                    BranchName = l.LibraryBranch != null ? l.LibraryBranch.BranchName : null,
                    LoanDate = l.LoanDate,
                    DueDate = l.DueDate,
                    ReturnDate = l.ReturnDate,
                    Status = l.Status,
                    FineAmount = l.FineAmount,
                    IsFinePaid = l.IsFinePaid
                })
                .ToListAsync();

            var viewModel = new BookLoanListViewModel
            {
                Loans = loans,
                SearchTerm = searchTerm,
                StatusFilter = statusFilter,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return View(viewModel);
        }

        // GET: BookLoans/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new BookLoanCreateViewModel
            {
                LoanDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(14),
                Books = new SelectList(await _context.Books
                    .Where(b => b.AvailableCopies > 0)
                    .Select(b => new { b.Id, Display = $"{b.Title} (ISBN: {b.ISBN})" })
                    .ToListAsync(), "Id", "Display"),
                Customers = new SelectList(await _context.Customers
                    .Where(c => c.IsActiveMember)
                    .Select(c => new { c.Id, Display = $"{c.FirstName} {c.LastName} ({c.Email})" })
                    .ToListAsync(), "Id", "Display"),
                LibraryBranches = new SelectList(await _context.LibraryBranches
                    .Where(b => b.IsActive)
                    .ToListAsync(), "Id", "BranchName")
            };
            return View(viewModel);
        }

        // POST: BookLoans/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookLoanCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Check if book is available
                var book = await _context.Books.FindAsync(viewModel.BookId);
                if (book == null || book.AvailableCopies <= 0)
                {
                    ModelState.AddModelError("BookId", "This book is not available for loan.");
                    await PopulateLoanDropdowns(viewModel);
                    return View(viewModel);
                }

                // Check if customer has overdue books
                var hasOverdue = await _context.BookLoans
                    .AnyAsync(l => l.CustomerId == viewModel.CustomerId && l.Status == LoanStatus.Overdue);
                if (hasOverdue)
                {
                    ModelState.AddModelError("CustomerId", "This customer has overdue books and cannot borrow new ones.");
                    await PopulateLoanDropdowns(viewModel);
                    return View(viewModel);
                }

                var loan = new BookLoan
                {
                    BookId = viewModel.BookId,
                    CustomerId = viewModel.CustomerId,
                    LibraryBranchId = viewModel.LibraryBranchId,
                    LoanDate = viewModel.LoanDate,
                    DueDate = viewModel.DueDate,
                    Status = LoanStatus.Active,
                    Notes = viewModel.Notes,
                    CreatedDate = DateTime.UtcNow
                };

                // Decrease available copies
                book.AvailableCopies--;

                _context.Add(loan);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Book loan created successfully!";
                return RedirectToAction(nameof(Index));
            }

            await PopulateLoanDropdowns(viewModel);
            return View(viewModel);
        }

        // GET: BookLoans/Return/5
        public async Task<IActionResult> Return(int? id)
        {
            if (id == null)
                return NotFound();

            var loan = await _context.BookLoans
                .Include(l => l.Book)
                .Include(l => l.Customer)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null || loan.Status != LoanStatus.Active)
                return NotFound();

            var daysOverdue = (DateTime.UtcNow - loan.DueDate).Days;
            var fineAmount = daysOverdue > 0 ? daysOverdue * 0.50m : 0;

            var viewModel = new BookLoanReturnViewModel
            {
                Id = loan.Id,
                BookTitle = loan.Book?.Title ?? "Unknown",
                CustomerName = loan.Customer != null ? $"{loan.Customer.FirstName} {loan.Customer.LastName}" : "Unknown",
                LoanDate = loan.LoanDate,
                DueDate = loan.DueDate,
                ReturnDate = DateTime.UtcNow,
                FineAmount = fineAmount
            };

            return View(viewModel);
        }

        // POST: BookLoans/Return/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int id, BookLoanReturnViewModel viewModel)
        {
            if (id != viewModel.Id)
                return NotFound();

            var loan = await _context.BookLoans
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null)
                return NotFound();

            loan.ReturnDate = viewModel.ReturnDate;
            loan.Status = LoanStatus.Returned;
            loan.FineAmount = viewModel.FineAmount;
            loan.Notes = viewModel.Notes;
            loan.UpdatedDate = DateTime.UtcNow;

            // Increase available copies
            if (loan.Book != null)
            {
                loan.Book.AvailableCopies++;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Book returned successfully!";
            return RedirectToAction(nameof(Index));
        }

        // POST: BookLoans/MarkOverdue
        [HttpPost]
        public async Task<IActionResult> MarkOverdue()
        {
            var overdueLoans = await _context.BookLoans
                .Where(l => l.Status == LoanStatus.Active && l.DueDate < DateTime.UtcNow)
                .ToListAsync();

            foreach (var loan in overdueLoans)
            {
                loan.Status = LoanStatus.Overdue;
                var daysOverdue = (DateTime.UtcNow - loan.DueDate).Days;
                loan.FineAmount = daysOverdue * 0.50m;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"{overdueLoans.Count} loans marked as overdue.";
            return RedirectToAction(nameof(Index));
        }

        // GET: BookLoans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var loan = await _context.BookLoans
                .Include(l => l.Book)
                .Include(l => l.Customer)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null)
                return NotFound();

            return View(loan);
        }

        // POST: BookLoans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loan = await _context.BookLoans.Include(l => l.Book).FirstOrDefaultAsync(l => l.Id == id);
            if (loan != null)
            {
                // If loan was active, return the book
                if (loan.Status == LoanStatus.Active && loan.Book != null)
                {
                    loan.Book.AvailableCopies++;
                }

                _context.BookLoans.Remove(loan);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Loan record deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateLoanDropdowns(BookLoanCreateViewModel viewModel)
        {
            viewModel.Books = new SelectList(await _context.Books
                .Where(b => b.AvailableCopies > 0)
                .Select(b => new { b.Id, Display = $"{b.Title} (ISBN: {b.ISBN})" })
                .ToListAsync(), "Id", "Display");
            viewModel.Customers = new SelectList(await _context.Customers
                .Where(c => c.IsActiveMember)
                .Select(c => new { c.Id, Display = $"{c.FirstName} {c.LastName} ({c.Email})" })
                .ToListAsync(), "Id", "Display");
            viewModel.LibraryBranches = new SelectList(await _context.LibraryBranches
                .Where(b => b.IsActive)
                .ToListAsync(), "Id", "BranchName");
        }
    }
}