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

            // Search by title or ISBN
            if (!string.IsNullOrEmpty(searchString))
            {
                booksQuery = booksQuery.Where(b => 
                    b.Title.Contains(searchString) || 
                    b.ISBN.Contains(searchString) ||
                    (b.Author != null && (b.Author.FirstName.Contains(searchString) || b.Author.LastName.Contains(searchString))));
            }

            // Filter by genre
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

            // Get list of genres for filter dropdown
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
    }
}