using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class CustomersController : Controller
    {
        private readonly LibraryDbContext _context;

        public CustomersController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index(string? searchTerm, bool? activeOnly, string? sortBy, int pageNumber = 1)
        {
            var query = _context.Customers
                .Include(c => c.PreferredBranch)
                .Include(c => c.BookLoans)
                .AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c => c.FirstName.Contains(searchTerm) ||
                                         c.LastName.Contains(searchTerm) ||
                                         c.Email.Contains(searchTerm) ||
                                         (c.LibraryCardNumber != null && c.LibraryCardNumber.Contains(searchTerm)));
            }

            // Filter
            if (activeOnly == true)
                query = query.Where(c => c.IsActiveMember);

            // Sorting
            query = sortBy switch
            {
                "name_desc" => query.OrderByDescending(c => c.LastName).ThenByDescending(c => c.FirstName),
                "email" => query.OrderBy(c => c.Email),
                "newest" => query.OrderByDescending(c => c.MembershipDate),
                "oldest" => query.OrderBy(c => c.MembershipDate),
                _ => query.OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
            };

            var pageSize = 10;
            var totalCount = await query.CountAsync();

            var customers = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CustomerViewModel
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    Address = c.Address,
                    City = c.City,
                    PostalCode = c.PostalCode,
                    Country = c.Country,
                    MembershipDate = c.MembershipDate,
                    LibraryCardNumber = c.LibraryCardNumber,
                    IsActiveMember = c.IsActiveMember,
                    ProfileImageUrl = c.ProfileImageUrl,
                    PreferredBranchName = c.PreferredBranch != null ? c.PreferredBranch.BranchName : null,
                    ActiveLoansCount = c.BookLoans.Count(l => l.Status == LoanStatus.Active)
                })
                .ToListAsync();

            var viewModel = new CustomerListViewModel
            {
                Customers = customers,
                SearchTerm = searchTerm,
                SortBy = sortBy,
                ActiveOnly = activeOnly,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return View(viewModel);
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var customer = await _context.Customers
                .Include(c => c.PreferredBranch)
                .Include(c => c.BookLoans)
                    .ThenInclude(l => l.Book)
                .Include(c => c.Reviews)
                    .ThenInclude(r => r.Book)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                return NotFound();

            var viewModel = new CustomerDetailsViewModel
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                City = customer.City,
                PostalCode = customer.PostalCode,
                Country = customer.Country,
                DateOfBirth = customer.DateOfBirth,
                MembershipDate = customer.MembershipDate,
                MembershipExpiry = customer.MembershipExpiry,
                LibraryCardNumber = customer.LibraryCardNumber,
                IsActiveMember = customer.IsActiveMember,
                ProfileImageUrl = customer.ProfileImageUrl,
                PreferredBranchName = customer.PreferredBranch?.BranchName,
                PreferredBranchId = customer.PreferredBranchId,
                CreatedDate = customer.CreatedDate,
                UpdatedDate = customer.UpdatedDate,
                ActiveLoansCount = customer.BookLoans.Count(l => l.Status == LoanStatus.Active),
                RecentLoans = customer.BookLoans
                    .OrderByDescending(l => l.LoanDate)
                    .Take(10)
                    .Select(l => new BookLoanViewModel
                    {
                        Id = l.Id,
                        BookTitle = l.Book != null ? l.Book.Title : "Unknown",
                        LoanDate = l.LoanDate,
                        DueDate = l.DueDate,
                        ReturnDate = l.ReturnDate,
                        Status = l.Status,
                        FineAmount = l.FineAmount
                    }),
                Reviews = customer.Reviews
                    .OrderByDescending(r => r.CreatedDate)
                    .Take(5)
                    .Select(r => new ReviewViewModel
                    {
                        Id = r.Id,
                        BookTitle = r.Book != null ? r.Book.Title : "Unknown",
                        Rating = r.Rating,
                        Title = r.Title,
                        Content = r.Content,
                        CreatedDate = r.CreatedDate
                    })
            };

            return View(viewModel);
        }

        // GET: Customers/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new CustomerCreateViewModel
            {
                LibraryBranches = new SelectList(await _context.LibraryBranches.ToListAsync(), "Id", "BranchName")
            };
            return View(viewModel);
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Check for duplicate email
                if (await _context.Customers.AnyAsync(c => c.Email == viewModel.Email))
                {
                    ModelState.AddModelError("Email", "A customer with this email already exists.");
                    viewModel.LibraryBranches = new SelectList(await _context.LibraryBranches.ToListAsync(), "Id", "BranchName");
                    return View(viewModel);
                }

                var customer = new Customer
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    Email = viewModel.Email,
                    PhoneNumber = viewModel.PhoneNumber,
                    Address = viewModel.Address,
                    City = viewModel.City,
                    PostalCode = viewModel.PostalCode,
                    Country = viewModel.Country,
                    DateOfBirth = viewModel.DateOfBirth,
                    MembershipDate = DateTime.UtcNow,
                                        MembershipExpiry = viewModel.MembershipExpiry,
                    LibraryCardNumber = $"LIB-{DateTime.Now:yyyy}-{await _context.Customers.CountAsync() + 1:D4}",
                    IsActiveMember = viewModel.IsActiveMember,
                    ProfileImageUrl = viewModel.ProfileImageUrl,
                    PreferredBranchId = viewModel.PreferredBranchId,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Add(customer);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Customer created successfully!";
                return RedirectToAction(nameof(Index));
            }

            viewModel.LibraryBranches = new SelectList(await _context.LibraryBranches.ToListAsync(), "Id", "BranchName");
            return View(viewModel);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            var viewModel = new CustomerEditViewModel
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                City = customer.City,
                PostalCode = customer.PostalCode,
                Country = customer.Country,
                DateOfBirth = customer.DateOfBirth,
                MembershipExpiry = customer.MembershipExpiry,
                LibraryCardNumber = customer.LibraryCardNumber,
                IsActiveMember = customer.IsActiveMember,
                ProfileImageUrl = customer.ProfileImageUrl,
                PreferredBranchId = customer.PreferredBranchId,
                LibraryBranches = new SelectList(await _context.LibraryBranches.ToListAsync(), "Id", "BranchName")
            };

            return View(viewModel);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CustomerEditViewModel viewModel)
        {
            if (id != viewModel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Check for duplicate email (excluding current customer)
                    if (await _context.Customers.AnyAsync(c => c.Email == viewModel.Email && c.Id != id))
                    {
                        ModelState.AddModelError("Email", "A customer with this email already exists.");
                        viewModel.LibraryBranches = new SelectList(await _context.LibraryBranches.ToListAsync(), "Id", "BranchName");
                        return View(viewModel);
                    }

                    var customer = await _context.Customers.FindAsync(id);
                    if (customer == null)
                        return NotFound();

                    customer.FirstName = viewModel.FirstName;
                    customer.LastName = viewModel.LastName;
                    customer.Email = viewModel.Email;
                    customer.PhoneNumber = viewModel.PhoneNumber;
                    customer.Address = viewModel.Address;
                    customer.City = viewModel.City;
                    customer.PostalCode = viewModel.PostalCode;
                    customer.Country = viewModel.Country;
                    customer.DateOfBirth = viewModel.DateOfBirth;
                    customer.MembershipExpiry = viewModel.MembershipExpiry;
                    customer.IsActiveMember = viewModel.IsActiveMember;
                    customer.ProfileImageUrl = viewModel.ProfileImageUrl;
                    customer.PreferredBranchId = viewModel.PreferredBranchId;
                    customer.UpdatedDate = DateTime.UtcNow;

                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Customer updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(viewModel.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            viewModel.LibraryBranches = new SelectList(await _context.LibraryBranches.ToListAsync(), "Id", "BranchName");
            return View(viewModel);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var customer = await _context.Customers
                .Include(c => c.PreferredBranch)
                .Include(c => c.BookLoans)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                return NotFound();

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.BookLoans)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer != null)
            {
                if (customer.BookLoans.Any(l => l.Status == LoanStatus.Active))
                {
                    TempData["Error"] = "Cannot delete customer with active loans.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Customer deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}