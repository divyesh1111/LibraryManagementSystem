using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibraryManagementSystem.UnitTests
{
    public class ReviewServiceTests
    {
        private static LibraryDbContext CreateDb()
        {
            var opts = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
                .Options;
            return new LibraryDbContext(opts);
        }

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenCustomerDidNotBorrowBook()
        {
            using var db = CreateDb();
            var svc = new ReviewService(db);

            db.Books.Add(new Book { Title = "T", ISBN = "X1", AuthorId = 1, AvailableCopies = 1, TotalCopies = 1 });
            db.Authors.Add(new Author { FirstName = "A", LastName = "B" });
            db.Customers.Add(new Customer { FirstName = "C", LastName = "D", Email = "me@test.com" });
            await db.SaveChangesAsync();

            var review = new Review { BookId = 1, CustomerId = 1, Rating = 5, Content = "This is a valid long review." };

            var (ok, reason, created) = await svc.CreateAsync(review, "me@test.com");

            Assert.False(ok);
            Assert.Null(created);
            Assert.Contains("borrow", reason ?? "");
        }

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenCustomerEmailDoesNotMatchTokenEmail()
        {
            using var db = CreateDb();
            var svc = new ReviewService(db);

            db.Authors.Add(new Author { FirstName = "A", LastName = "B" });
            db.Books.Add(new Book { Title = "T", ISBN = "X2", AuthorId = 1, AvailableCopies = 1, TotalCopies = 1 });
            db.Customers.Add(new Customer { FirstName = "C", LastName = "D", Email = "real@test.com" });
            db.BookLoans.Add(new BookLoan { BookId = 1, CustomerId = 1, DueDate = DateTime.UtcNow.AddDays(7), Status = LoanStatus.Active });
            await db.SaveChangesAsync();

            var review = new Review { BookId = 1, CustomerId = 1, Rating = 5, Content = "This is a valid long review." };

            var (ok, reason, created) = await svc.CreateAsync(review, "hacker@test.com");

            Assert.False(ok);
            Assert.Null(created);
            Assert.Contains("email mismatch", (reason ?? "").ToLowerInvariant());
        }
    }
}