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
        public async Task<IActionResult> Index(string searchString, string genre)
        {
            var booksQuery = _context.Books
                .Include(b => b.Author)
                .Include(b => b.LibraryBranch)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                booksQuery = booksQuery.Where(b => 
                    b.Title.Contains(searchString) || 
                    b.ISBN.Contains(searchString) ||
                    (b.Author != null && (b.Author.FirstName.Contains(searchString) || b.Author.LastName.Contains(searchString))));
            }

            if (!string.IsNullOrEmpty(genre))
            {
                booksQuery = booksQuery.Where(b => b.Genre == genre);
            }

            var books = await booksQuery
                .OrderBy(b => b.Title)
                .Select(b => new BookViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    ISBN = b.ISBN,
                    Description = b.Description,
                    PublicationDate = b.PublicationDate,
                    Publisher = b.Publisher,
                    PageCount = b.PageCount,
                    Genre = b.Genre,
                    CoverImageUrl = b.CoverImageUrl,
                    AvailableCopies = b.AvailableCopies,
                    TotalCopies = b.TotalCopies,
                    AuthorId = b.AuthorId,
                    AuthorName = b.Author != null ? $"{b.Author.FirstName} {b.Author.LastName}" : "Unknown",
                    LibraryBranchId = b.LibraryBranchId,
                    LibraryBranchName = b.LibraryBranch != null ? b.LibraryBranch.BranchName : null
                })
                .ToListAsync();

            ViewBag.Genres = await _context.Books
                .Where(b => b.Genre != null)
                .Select(b => b.Genre)
                .Distinct()
                .OrderBy(g => g)
                .ToListAsync();

            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentGenre = genre;

            return View(books);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.LibraryBranch)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            var viewModel = new BookDetailsViewModel
            {
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                Description = book.Description,
                PublicationDate = book.PublicationDate,
                Publisher = book.Publisher,
                PageCount = book.PageCount,
                Genre = book.Genre,
                CoverImageUrl = book.CoverImageUrl,
                AvailableCopies = book.AvailableCopies,
                TotalCopies = book.TotalCopies,
                CreatedDate = book.CreatedDate,
                AuthorId = book.AuthorId,
                AuthorName = book.Author != null ? $"{book.Author.FirstName} {book.Author.LastName}" : "Unknown",
                LibraryBranchId = book.LibraryBranchId,
                LibraryBranchName = book.LibraryBranch?.BranchName
            };

            return View(viewModel);
        }

        // GET: Books/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new BookCreateViewModel
            {
                Authors = new SelectList(
                    await _context.Authors
                        .OrderBy(a => a.LastName)
                        .ThenBy(a => a.FirstName)
                        .Select(a => new { a.Id, FullName = a.FirstName + " " + a.LastName })
                        .ToListAsync(),
                    "Id", "FullName"),
                LibraryBranches = new SelectList(
                    await _context.LibraryBranches
                        .Where(b => b.IsActive)
                        .OrderBy(b => b.BranchName)
                        .ToListAsync(),
                    "Id", "BranchName")
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
                // Check if ISBN already exists
                if (await _context.Books.AnyAsync(b => b.ISBN == viewModel.ISBN))
                {
                    ModelState.AddModelError("ISBN", "A book with this ISBN already exists.");
                }
                else
                {
                    var book = new Book
                    {
                        Title = viewModel.Title,
                        ISBN = viewModel.ISBN,
                        Description = viewModel.Description,
                        PublicationDate = viewModel.PublicationDate,
                        Publisher = viewModel.Publisher,
                        PageCount = viewModel.PageCount,
                        Genre = viewModel.Genre,
                        CoverImageUrl = viewModel.CoverImageUrl,
                        AvailableCopies = viewModel.AvailableCopies,
                        TotalCopies = viewModel.TotalCopies,
                        AuthorId = viewModel.AuthorId,
                        LibraryBranchId = viewModel.LibraryBranchId,
                        CreatedDate = DateTime.UtcNow
                    };

                    _context.Add(book);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Book created successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }

            // Reload dropdown lists
            viewModel.Authors = new SelectList(
                await _context.Authors
                    .OrderBy(a => a.LastName)
                    .ThenBy(a => a.FirstName)
                    .Select(a => new { a.Id, FullName = a.FirstName + " " + a.LastName })
                    .ToListAsync(),
                "Id", "FullName", viewModel.AuthorId);
            viewModel.LibraryBranches = new SelectList(
                await _context.LibraryBranches
                    .Where(b => b.IsActive)
                    .OrderBy(b => b.BranchName)
                    .ToListAsync(),
                "Id", "BranchName", viewModel.LibraryBranchId);

            return View(viewModel);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            var viewModel = new BookEditViewModel
            {
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                Description = book.Description,
                PublicationDate = book.PublicationDate,
                Publisher = book.Publisher,
                PageCount = book.PageCount,
                Genre = book.Genre,
                CoverImageUrl = book.CoverImageUrl,
                AvailableCopies = book.AvailableCopies,
                TotalCopies = book.TotalCopies,
                AuthorId = book.AuthorId,
                LibraryBranchId = book.LibraryBranchId,
                Authors = new SelectList(
                    await _context.Authors
                        .OrderBy(a => a.LastName)
                        .ThenBy(a => a.FirstName)
                        .Select(a => new { a.Id, FullName = a.FirstName + " " + a.LastName })
                        .ToListAsync(),
                    "Id", "FullName", book.AuthorId),
                LibraryBranches = new SelectList(
                    await _context.LibraryBranches
                        .Where(b => b.IsActive)
                        .OrderBy(b => b.BranchName)
                        .ToListAsync(),
                    "Id", "BranchName", book.LibraryBranchId)
            };

            return View(viewModel);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Check if ISBN already exists for another book
                if (await _context.Books.AnyAsync(b => b.ISBN == viewModel.ISBN && b.Id != id))
                {
                    ModelState.AddModelError("ISBN", "A book with this ISBN already exists.");
                }
                else
                {
                    try
                    {
                        var book = await _context.Books.FindAsync(id);
                        if (book == null)
                        {
                            return NotFound();
                        }

                        book.Title = viewModel.Title;
                        book.ISBN = viewModel.ISBN;
                        book.Description = viewModel.Description;
                        book.PublicationDate = viewModel.PublicationDate;
                        book.Publisher = viewModel.Publisher;
                        book.PageCount = viewModel.PageCount;
                        book.Genre = viewModel.Genre;
                        book.CoverImageUrl = viewModel.CoverImageUrl;
                        book.AvailableCopies = viewModel.AvailableCopies;
                        book.TotalCopies = viewModel.TotalCopies;
                        book.AuthorId = viewModel.AuthorId;
                        book.LibraryBranchId = viewModel.LibraryBranchId;

                        _context.Update(book);
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Book updated successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!await BookExists(viewModel.Id))
                        {
                            return NotFound();
                        }
                        throw;
                    }
                }
            }

            // Reload dropdown lists
            viewModel.Authors = new SelectList(
                await _context.Authors
                    .OrderBy(a => a.LastName)
                    .ThenBy(a => a.FirstName)
                    .Select(a => new { a.Id, FullName = a.FirstName + " " + a.LastName })
                    .ToListAsync(),
                "Id", "FullName", viewModel.AuthorId);
            viewModel.LibraryBranches = new SelectList(
                await _context.LibraryBranches
                    .Where(b => b.IsActive)
                    .OrderBy(b => b.BranchName)
                    .ToListAsync(),
                "Id", "BranchName", viewModel.LibraryBranchId);

            return View(viewModel);
        }

        private async Task<bool> BookExists(int id)
        {
            return await _context.Books.AnyAsync(e => e.Id == id);
        }
    }
}