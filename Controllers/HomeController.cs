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
            var dashboard = new DashboardViewModel
            {
                TotalBooks = await _context.Books.CountAsync(),
                TotalAuthors = await _context.Authors.CountAsync(),
                TotalCustomers = await _context.Customers.CountAsync(),
                TotalBranches = await _context.LibraryBranches.CountAsync(),
                TotalCategories = await _context.Categories.CountAsync(),
                ActiveLoans = await _context.BookLoans.CountAsync(l => l.Status == LoanStatus.Active),
                OverdueLoans = await _context.BookLoans.CountAsync(l => l.Status == LoanStatus.Overdue),
                TotalReviews = await _context.Reviews.CountAsync(),
                AvailableBooks = await _context.Books.SumAsync(b => b.AvailableCopies),

                RecentBooks = await _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Category)
                    .OrderByDescending(b => b.CreatedDate)
                    .Take(6)
                    .Select(b => new BookViewModel
                    {
                        Id = b.Id,
                        Title = b.Title,
                        ISBN = b.ISBN,
                        CoverImageUrl = b.CoverImageUrl,
                        AuthorName = b.Author != null ? $"{b.Author.FirstName} {b.Author.LastName}" : "Unknown",
                        CategoryName = b.Category != null ? b.Category.Name : null,
                        AvailableCopies = b.AvailableCopies,
                        Price = b.Price
                    })
                    .ToListAsync(),

                RecentLoans = await _context.BookLoans
                    .Include(l => l.Book)
                    .Include(l => l.Customer)
                    .OrderByDescending(l => l.LoanDate)
                    .Take(5)
                    .Select(l => new BookLoanViewModel
                    {
                        Id = l.Id,
                        BookTitle = l.Book != null ? l.Book.Title : "Unknown",
                        CustomerName = l.Customer != null ? $"{l.Customer.FirstName} {l.Customer.LastName}" : "Unknown",
                        LoanDate = l.LoanDate,
                        DueDate = l.DueDate,
                        Status = l.Status
                    })
                    .ToListAsync(),

                NewCustomers = await _context.Customers
                    .OrderByDescending(c => c.MembershipDate)
                    .Take(5)
                    .Select(c => new CustomerViewModel
                    {
                        Id = c.Id,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        Email = c.Email,
                        MembershipDate = c.MembershipDate,
                        IsActiveMember = c.IsActiveMember
                    })
                    .ToListAsync()
            };

            // Get books by category
            dashboard.BooksByCategory = await _context.Categories
                .Select(c => new { c.Name, Count = c.Books.Count })
                .ToDictionaryAsync(x => x.Name, x => x.Count);

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