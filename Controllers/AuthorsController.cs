using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly LibraryDbContext _context;

        public AuthorsController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Authors
        public async Task<IActionResult> Index(string? searchTerm, string? sortBy, int pageNumber = 1)
        {
            var query = _context.Authors
                .Include(a => a.Books)
                .AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a => a.FirstName.Contains(searchTerm) || 
                                         a.LastName.Contains(searchTerm) ||
                                         (a.Nationality != null && a.Nationality.Contains(searchTerm)));
            }

            // Sorting
            query = sortBy switch
            {
                "name_desc" => query.OrderByDescending(a => a.LastName).ThenByDescending(a => a.FirstName),
                "books" => query.OrderByDescending(a => a.Books.Count),
                "newest" => query.OrderByDescending(a => a.CreatedDate),
                _ => query.OrderBy(a => a.LastName).ThenBy(a => a.FirstName)
            };

            var pageSize = 10;
            var totalCount = await query.CountAsync();

            var authors = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AuthorViewModel
                {
                    Id = a.Id,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    DateOfBirth = a.DateOfBirth,
                    Nationality = a.Nationality,
                    Biography = a.Biography,
                    WebsiteUrl = a.WebsiteUrl,
                    Email = a.Email,
                    ImageUrl = a.ImageUrl,
                    IsActive = a.IsActive,
                    BookCount = a.Books.Count
                })
                .ToListAsync();

            var viewModel = new AuthorListViewModel
            {
                Authors = authors,
                SearchTerm = searchTerm,
                SortBy = sortBy,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return View(viewModel);
        }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var author = await _context.Authors
                .Include(a => a.Books)
                    .ThenInclude(b => b.Category)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
                return NotFound();

            var viewModel = new AuthorDetailsViewModel
            {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName,
                DateOfBirth = author.DateOfBirth,
                Nationality = author.Nationality,
                Biography = author.Biography,
                WebsiteUrl = author.WebsiteUrl,
                Email = author.Email,
                ImageUrl = author.ImageUrl,
                IsActive = author.IsActive,
                BookCount = author.Books.Count,
                CreatedDate = author.CreatedDate,
                UpdatedDate = author.UpdatedDate,
                Books = author.Books.Select(b => new BookViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    ISBN = b.ISBN,
                    PublicationDate = b.PublicationDate,
                    CoverImageUrl = b.CoverImageUrl,
                    CategoryName = b.Category?.Name,
                    AvailableCopies = b.AvailableCopies
                }).ToList()
            };

            return View(viewModel);
        }

        // GET: Authors/Create
        public IActionResult Create()
        {
            return View(new AuthorCreateViewModel());
        }

        // POST: Authors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AuthorCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var author = new Author
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    DateOfBirth = viewModel.DateOfBirth,
                    Nationality = viewModel.Nationality,
                    Biography = viewModel.Biography,
                    WebsiteUrl = viewModel.WebsiteUrl,
                    Email = viewModel.Email,
                    ImageUrl = viewModel.ImageUrl,
                    IsActive = viewModel.IsActive,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Add(author);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Author created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var author = await _context.Authors.FindAsync(id);
            if (author == null)
                return NotFound();

            var viewModel = new AuthorEditViewModel
            {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName,
                DateOfBirth = author.DateOfBirth,
                Nationality = author.Nationality,
                Biography = author.Biography,
                WebsiteUrl = author.WebsiteUrl,
                Email = author.Email,
                ImageUrl = author.ImageUrl,
                IsActive = author.IsActive
            };

            return View(viewModel);
        }

        // POST: Authors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AuthorEditViewModel viewModel)
        {
            if (id != viewModel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var author = await _context.Authors.FindAsync(id);
                    if (author == null)
                        return NotFound();

                    author.FirstName = viewModel.FirstName;
                    author.LastName = viewModel.LastName;
                    author.DateOfBirth = viewModel.DateOfBirth;
                    author.Nationality = viewModel.Nationality;
                    author.Biography = viewModel.Biography;
                    author.WebsiteUrl = viewModel.WebsiteUrl;
                    author.Email = viewModel.Email;
                    author.ImageUrl = viewModel.ImageUrl;
                    author.IsActive = viewModel.IsActive;
                    author.UpdatedDate = DateTime.UtcNow;

                    _context.Update(author);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Author updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorExists(viewModel.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Authors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
                return NotFound();

            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var author = await _context.Authors.Include(a => a.Books).FirstOrDefaultAsync(a => a.Id == id);
            if (author != null)
            {
                if (author.Books.Any())
                {
                    TempData["Error"] = "Cannot delete author with associated books. Please reassign or delete the books first.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Author deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }
    }
}