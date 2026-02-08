using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class LibraryBranchesController : Controller
    {
        private readonly LibraryDbContext _context;

        public LibraryBranchesController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: LibraryBranches
        public async Task<IActionResult> Index(string? searchTerm, bool? activeOnly, string? sortBy, int pageNumber = 1)
        {
            var query = _context.LibraryBranches
                .Include(b => b.Books)
                .Include(b => b.Customers)
                .Include(b => b.BookLoans)
                .AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(b => b.BranchName.Contains(searchTerm) ||
                                         b.City!.Contains(searchTerm) ||
                                         b.Address.Contains(searchTerm));
            }

            // Filter
            if (activeOnly == true)
                query = query.Where(b => b.IsActive);

            // Sorting
            query = sortBy switch
            {
                "name_desc" => query.OrderByDescending(b => b.BranchName),
                "city" => query.OrderBy(b => b.City),
                "books" => query.OrderByDescending(b => b.Books.Count),
                "newest" => query.OrderByDescending(b => b.EstablishedDate),
                _ => query.OrderBy(b => b.BranchName)
            };

            var pageSize = 10;
            var totalCount = await query.CountAsync();

            var branches = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new LibraryBranchViewModel
                {
                    Id = b.Id,
                    BranchName = b.BranchName,
                    Address = b.Address,
                    City = b.City,
                    State = b.State,
                    PostalCode = b.PostalCode,
                    Country = b.Country,
                    PhoneNumber = b.PhoneNumber,
                    Email = b.Email,
                    OpeningHours = b.OpeningHours,
                    ManagerName = b.ManagerName,
                    IsActive = b.IsActive,
                    ImageUrl = b.ImageUrl,
                    BookCount = b.Books.Count,
                    CustomerCount = b.Customers.Count,
                    ActiveLoansCount = b.BookLoans.Count(l => l.Status == LoanStatus.Active)
                })
                .ToListAsync();

            var viewModel = new LibraryBranchListViewModel
            {
                Branches = branches,
                SearchTerm = searchTerm,
                SortBy = sortBy,
                ActiveOnly = activeOnly,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return View(viewModel);
        }

        // GET: LibraryBranches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var branch = await _context.LibraryBranches
                .Include(b => b.Books)
                    .ThenInclude(book => book.Author)
                .Include(b => b.Customers)
                .Include(b => b.BookLoans)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (branch == null)
                return NotFound();

            var viewModel = new LibraryBranchDetailsViewModel
            {
                Id = branch.Id,
                BranchName = branch.BranchName,
                Address = branch.Address,
                City = branch.City,
                State = branch.State,
                PostalCode = branch.PostalCode,
                Country = branch.Country,
                PhoneNumber = branch.PhoneNumber,
                Email = branch.Email,
                Website = branch.Website,
                OpeningHours = branch.OpeningHours,
                ManagerName = branch.ManagerName,
                EstablishedDate = branch.EstablishedDate,
                IsActive = branch.IsActive,
                ImageUrl = branch.ImageUrl,
                TotalCapacity = branch.TotalCapacity,
                Description = branch.Description,
                Latitude = branch.Latitude,
                Longitude = branch.Longitude,
                BookCount = branch.Books.Count,
                CustomerCount = branch.Customers.Count,
                ActiveLoansCount = branch.BookLoans.Count(l => l.Status == LoanStatus.Active),
                CreatedDate = branch.CreatedDate,
                UpdatedDate = branch.UpdatedDate,
                Books = branch.Books.Take(10).Select(b => new BookViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    ISBN = b.ISBN,
                    AuthorName = b.Author != null ? $"{b.Author.FirstName} {b.Author.LastName}" : "Unknown",
                    CoverImageUrl = b.CoverImageUrl,
                    AvailableCopies = b.AvailableCopies
                })
            };

            return View(viewModel);
        }

        // GET: LibraryBranches/Create
        public IActionResult Create()
        {
            return View(new LibraryBranchCreateViewModel());
        }

        // POST: LibraryBranches/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LibraryBranchCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var branch = new LibraryBranch
                {
                    BranchName = viewModel.BranchName,
                    Address = viewModel.Address,
                    City = viewModel.City,
                    State = viewModel.State,
                    PostalCode = viewModel.PostalCode,
                    Country = viewModel.Country,
                    PhoneNumber = viewModel.PhoneNumber,
                    Email = viewModel.Email,
                    Website = viewModel.Website,
                    OpeningHours = viewModel.OpeningHours,
                    ManagerName = viewModel.ManagerName,
                    EstablishedDate = viewModel.EstablishedDate,
                    IsActive = viewModel.IsActive,
                    ImageUrl = viewModel.ImageUrl,
                    TotalCapacity = viewModel.TotalCapacity,
                    Description = viewModel.Description,
                    Latitude = viewModel.Latitude,
                    Longitude = viewModel.Longitude,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Add(branch);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Library branch created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: LibraryBranches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var branch = await _context.LibraryBranches.FindAsync(id);
            if (branch == null)
                return NotFound();

            var viewModel = new LibraryBranchEditViewModel
            {
                Id = branch.Id,
                BranchName = branch.BranchName,
                Address = branch.Address,
                City = branch.City,
                State = branch.State,
                PostalCode = branch.PostalCode,
                Country = branch.Country,
                PhoneNumber = branch.PhoneNumber,
                Email = branch.Email,
                Website = branch.Website,
                OpeningHours = branch.OpeningHours,
                ManagerName = branch.ManagerName,
                EstablishedDate = branch.EstablishedDate,
                IsActive = branch.IsActive,
                ImageUrl = branch.ImageUrl,
                TotalCapacity = branch.TotalCapacity,
                Description = branch.Description,
                Latitude = branch.Latitude,
                Longitude = branch.Longitude
            };

            return View(viewModel);
        }

        // POST: LibraryBranches/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LibraryBranchEditViewModel viewModel)
        {
            if (id != viewModel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var branch = await _context.LibraryBranches.FindAsync(id);
                    if (branch == null)
                        return NotFound();

                    branch.BranchName = viewModel.BranchName;
                    branch.Address = viewModel.Address;
                    branch.City = viewModel.City;
                    branch.State = viewModel.State;
                    branch.PostalCode = viewModel.PostalCode;
                    branch.Country = viewModel.Country;
                    branch.PhoneNumber = viewModel.PhoneNumber;
                    branch.Email = viewModel.Email;
                    branch.Website = viewModel.Website;
                    branch.OpeningHours = viewModel.OpeningHours;
                    branch.ManagerName = viewModel.ManagerName;
                    branch.EstablishedDate = viewModel.EstablishedDate;
                    branch.IsActive = viewModel.IsActive;
                    branch.ImageUrl = viewModel.ImageUrl;
                    branch.TotalCapacity = viewModel.TotalCapacity;
                    branch.Description = viewModel.Description;
                    branch.Latitude = viewModel.Latitude;
                    branch.Longitude = viewModel.Longitude;
                    branch.UpdatedDate = DateTime.UtcNow;

                    _context.Update(branch);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Library branch updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibraryBranchExists(viewModel.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: LibraryBranches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var branch = await _context.LibraryBranches
                .Include(b => b.Books)
                .Include(b => b.Customers)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (branch == null)
                return NotFound();

            return View(branch);
        }

        // POST: LibraryBranches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var branch = await _context.LibraryBranches
                .Include(b => b.Books)
                .Include(b => b.BookLoans)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (branch != null)
            {
                if (branch.Books.Any() || branch.BookLoans.Any(l => l.Status == LoanStatus.Active))
                {
                    TempData["Error"] = "Cannot delete branch with books or active loans.";
                    return RedirectToAction(nameof(Index));
                }

                _context.LibraryBranches.Remove(branch);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Library branch deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool LibraryBranchExists(int id)
        {
            return _context.LibraryBranches.Any(e => e.Id == id);
        }
    }
}