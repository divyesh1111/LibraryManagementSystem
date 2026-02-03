using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace LibraryManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly LibraryDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(LibraryDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var totalBooks = await _context.Books.SumAsync(b => b.TotalCopies);
            var availableBooks = await _context.Books.SumAsync(b => b.AvailableCopies);

            var dashboard = new DashboardViewModel
            {
                TotalBooks = await _context.Books.CountAsync(),
                TotalAuthors = await _context.Authors.CountAsync(),
                TotalCustomers = await _context.Customers.CountAsync(),
                TotalBranches = await _context.LibraryBranches.CountAsync(),
                AvailableBooks = availableBooks,
                BorrowedBooks = totalBooks - availableBooks,

                RecentBooks = await _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.LibraryBranch)
                    .OrderByDescending(b => b.CreatedDate)
                    .Take(5)
                    .Select(b => new BookViewModel
                    {
                        Id = b.Id,
                        Title = b.Title,
                        ISBN = b.ISBN,
                        Genre = b.Genre,
                        CoverImageUrl = b.CoverImageUrl,
                        AuthorId = b.AuthorId,
                        AuthorName = b.Author != null ? $"{b.Author.FirstName} {b.Author.LastName}" : "Unknown",
                        LibraryBranchId = b.LibraryBranchId,
                        LibraryBranchName = b.LibraryBranch != null ? b.LibraryBranch.BranchName : null,
                        AvailableCopies = b.AvailableCopies,
                        TotalCopies = b.TotalCopies
                    })
                    .ToListAsync(),

                TopAuthors = await _context.Authors
                    .Include(a => a.Books)
                    .OrderByDescending(a => a.Books.Count)
                    .Take(5)
                    .Select(a => new AuthorViewModel
                    {
                        Id = a.Id,
                        FirstName = a.FirstName,
                        LastName = a.LastName,
                        Nationality = a.Nationality,
                        IsActive = a.IsActive,
                        BookCount = a.Books.Count
                    })
                    .ToListAsync(),

                RecentCustomers = await _context.Customers
                    .Include(c => c.PreferredBranch)
                    .OrderByDescending(c => c.MembershipDate)
                    .Take(5)
                    .Select(c => new CustomerViewModel
                    {
                        Id = c.Id,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        Email = c.Email,
                        PhoneNumber = c.PhoneNumber,
                        City = c.City,
                        MembershipDate = c.MembershipDate,
                        LibraryCardNumber = c.LibraryCardNumber,
                        IsActiveMember = c.IsActiveMember,
                        PreferredBranchName = c.PreferredBranch != null ? c.PreferredBranch.BranchName : null
                    })
                    .ToListAsync()
            };

            return View(dashboard);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}