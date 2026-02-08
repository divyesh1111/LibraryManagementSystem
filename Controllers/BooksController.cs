using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class BooksController : Controller
    {
        private readonly LibraryDbContext _context;

        public BooksController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index(string? searchTerm, int? categoryFilter, int? authorFilter, int? branchFilter, bool? availableOnly, string? sortBy, int pageNumber = 1)
        {
            var query = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.LibraryBranch)
                .AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(b => b.Title.Contains(searchTerm) || 
                                         b.ISBN.Contains(searchTerm) ||
                                         (b.Author != null && (b.Author.FirstName.Contains(searchTerm) || b.Author.LastName.Contains(searchTerm))));
            }

            // Filters
            if (categoryFilter.HasValue)
                query = query.Where(b => b.CategoryId == categoryFilter);

            if (authorFilter.HasValue)
                query = query.Where(b => b.AuthorId == authorFilter);

            if (branchFilter.HasValue)
                query = query.Where(b => b.LibraryBranchId == branchFilter);

            if (availableOnly == true)
                query = query.Where(b => b.AvailableCopies > 0);

            // Sorting
            query = sortBy switch
            {
                "title_desc" => query.OrderByDescending(b => b.Title),
                "author" => query.OrderBy(b => b.Author!.LastName),
                "newest" => query.OrderByDescending(b => b.PublicationDate),
                "oldest" => query.OrderBy(b => b.PublicationDate),
                _ => query.OrderBy(b => b.Title)
            };

            var pageSize = 12;
            var totalCount = await query.CountAsync();

            var books = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    ISBN = b.ISBN,
                    Description = b.Description,
                    PublicationDate = b.PublicationDate,
                    Publisher = b.Publisher,
                    PageCount = b.PageCount,
                    Language = b.Language,
                    CoverImageUrl = b.CoverImageUrl,
                    Price = b.Price,
                    AvailableCopies = b.AvailableCopies,
                    TotalCopies = b.TotalCopies,
                    AuthorId = b.AuthorId,
                    AuthorName = b.Author != null ? $"{b.Author.FirstName} {b.Author.LastName}" : "Unknown",
                    CategoryId = b.CategoryId,
                    CategoryName = b.Category != null ? b.Category.Name : null,
                    LibraryBranchId = b.LibraryBranchId,
                    LibraryBranchName = b.LibraryBranch != null ? b.LibraryBranch.BranchName : null,
                    AverageRating = b.Reviews.Any() ? b.Reviews.Average(r => r.Rating) : 0,
                    ReviewCount = b.Reviews.Count
                })
                .ToListAsync();

            var viewModel = new BookListViewModel
            {
                Books = books,
                SearchTerm = searchTerm,
                SortBy = sortBy,
                CategoryFilter = categoryFilter,
                AuthorFilter = authorFilter,
                BranchFilter = branchFilter,
                AvailableOnly = availableOnly,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name"),
                Authors = new SelectList(await _context.Authors.Select(a => new { a.Id, Name = a.FirstName + " " + a.LastName }).ToListAsync(), "Id", "Name"),
                Branches = new SelectList(await _context.LibraryBranches.ToListAsync(), "Id", "BranchName")
            };

            return View(viewModel);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.LibraryBranch)
                .Include(b => b.Reviews)
                    .ThenInclude(r => r.Customer)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            var viewModel = new BookDetailsViewModel
            {
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                Description = book.Description,
                PublicationDate = book.PublicationDate,
                Publisher = book.Publisher,
                PageCount = book.PageCount,
                Language = book.Language,
                CoverImageUrl = book.CoverImageUrl,
                Price = book.Price,
                AvailableCopies = book.AvailableCopies,
                TotalCopies = book.TotalCopies,
                AuthorId = book.AuthorId,
                AuthorName = book.Author != null ? $"{book.Author.FirstName} {book.Author.LastName}" : "Unknown",
                CategoryId = book.CategoryId,
                CategoryName = book.Category?.Name,
                LibraryBranchId = book.LibraryBranchId,
                LibraryBranchName = book.LibraryBranch?.BranchName,
                Author = book.Author,
                Category = book.Category,
                LibraryBranch = book.LibraryBranch,
                Reviews = book.Reviews,
                AverageRating = book.Reviews.Any() ? book.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = book.Reviews.Count,
                CreatedDate = book.CreatedDate,
                UpdatedDate = book.UpdatedDate
            };

            return View(viewModel);
        }

        // GET: Books/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new BookCreateViewModel
            {
                Authors = new SelectList(await _context.Authors.Select(a => new { a.Id, Name = a.FirstName + " " + a.LastName }).ToListAsync(), "Id", "Name"),
                Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name"),
                LibraryBranches = new SelectList(await _context.LibraryBranches.ToListAsync(), "Id", "BranchName")
            };
            return View(viewModel);
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Check for duplicate ISBN
                if (await _context.Books.AnyAsync(b => b.ISBN == viewModel.ISBN))
                {
                    ModelState.AddModelError("ISBN", "A book with this ISBN already exists.");
                    viewModel.Authors = new SelectList(await _context.Authors.Select(a => new { a.Id, Name = a.FirstName + " " + a.LastName }).ToListAsync(), "Id", "Name");
                    viewModel.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
                    viewModel.LibraryBranches = new SelectList(await _context.LibraryBranches.ToListAsync(), "Id", "BranchName");
                    return View(viewModel);
                }

                var book = new Book
                {
                    Title = viewModel.Title,
                    ISBN = viewModel.ISBN,
                    Description = viewModel.Description,
                    PublicationDate = viewModel.PublicationDate,
                    Publisher = viewModel.Publisher,
                    PageCount = viewModel.PageCount,
                    Language = viewModel.Language,
                    CoverImageUrl = viewModel.CoverImageUrl,
                    Price = viewModel.Price,
                    AvailableCopies = viewModel.AvailableCopies,
                    TotalCopies = viewModel.TotalCopies,
                    AuthorId = viewModel.AuthorId,
                    CategoryId = viewModel.CategoryId,
                    LibraryBranchId = viewModel.LibraryBranchId,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Add(book);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Book created successfully!";
                return RedirectToAction(nameof(Index));
            }

            viewModel.Authors = new SelectList(await _context.Authors.Select(a => new { a.Id, Name = a.FirstName + " " + a.LastName }).ToListAsync(), "Id", "Name");
            viewModel.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            viewModel.LibraryBranches = new SelectList(await _context.LibraryBranches.ToListAsync(), "Id", "BranchName");
            return View(viewModel);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            var viewModel = new BookEditViewModel
            {
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                Description = book.Description,
                PublicationDate = book.PublicationDate,
                Publisher = book.Publisher,
                PageCount = book.PageCount,
                Language = book.Language,
                CoverImageUrl = book.CoverImageUrl,
                Price = book.Price,
                AvailableCopies = book.AvailableCopies,
                TotalCopies = book.TotalCopies,
                AuthorId = book.AuthorId,
                CategoryId = book.CategoryId,
                LibraryBranchId = book.LibraryBranchId,
                Authors = new SelectList(await _context.Authors.Select(a => new { a.Id, Name = a.FirstName + " " + a.LastName }).ToListAsync(), "Id", "Name"),
                Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name"),
                LibraryBranches = new SelectList(await _context.LibraryBranches.ToListAsync(), "Id", "BranchName")
            };

            return View(viewModel);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookEditViewModel viewModel)
        {
            if (id != viewModel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Check for duplicate ISBN (excluding current book)
                    if (await _context.Books.AnyAsync(b => b.ISBN == viewModel.ISBN && b.Id != id))
                    {
                        ModelState.AddModelError("ISBN", "A book with this ISBN already exists.");
                        viewModel.Authors = new SelectList(await _context.Authors.Select(a => new { a.Id, Name = a.FirstName + " " + a.LastName }).ToListAsync(), "Id", "Name");
                        viewModel.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
                        viewModel.LibraryBranches = new SelectList(await _context.LibraryBranches.ToListAsync(), "Id", "BranchName");
                        return View(viewModel);
                    }

                    var book = await _context.Books.FindAsync(id);
                    if (book == null)
                        return NotFound();

                    book.Title = viewModel.Title;
                    book.ISBN = viewModel.ISBN;
                    book.Description = viewModel.Description;
                    book.PublicationDate = viewModel.PublicationDate;
                    book.Publisher = viewModel.Publisher;
                    book.PageCount = viewModel.PageCount;
                    book.Language = viewModel.Language;
                    book.CoverImageUrl = viewModel.CoverImageUrl;
                    book.Price = viewModel.Price;
                    book.AvailableCopies = viewModel.AvailableCopies;
                    book.TotalCopies = viewModel.TotalCopies;
                    book.AuthorId = viewModel.AuthorId;
                    book.CategoryId = viewModel.CategoryId;
                    book.LibraryBranchId = viewModel.LibraryBranchId;
                    book.UpdatedDate = DateTime.UtcNow;

                    _context.Update(book);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Book updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(viewModel.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            viewModel.Authors = new SelectList(await _context.Authors.Select(a => new { a.Id, Name = a.FirstName + " " + a.LastName }).ToListAsync(), "Id", "Name");
            viewModel.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            viewModel.LibraryBranches = new SelectList(await _context.LibraryBranches.ToListAsync(), "Id", "BranchName");
            return View(viewModel);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                // Check if book has active loans
                var hasActiveLoans = await _context.BookLoans.AnyAsync(l => l.BookId == id && l.Status == LoanStatus.Active);
                if (hasActiveLoans)
                {
                    TempData["Error"] = "Cannot delete book with active loans.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Book deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}