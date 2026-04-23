using LibraryManagementSystem.Data;
using LibraryManagementSystem.Exceptions;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibraryManagementSystem.UnitTests
{
    public class BookLoanServiceTests
    {
        private static LibraryDbContext CreateDb()
        {
            var opts = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
                .Options;
            return new LibraryDbContext(opts);
        }

        private static async Task SeedBookAndCustomer(LibraryDbContext db)
        {
            db.Authors.Add(new Author { Id = 1, FirstName = "A", LastName = "B" });
            db.Books.Add(new Book
            {
                Id = 1,
                Title = "Test Book",
                ISBN = "T-001",
                AuthorId = 1,
                AvailableCopies = 3,
                TotalCopies = 3
            });
            db.Customers.Add(new Customer
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                IsActiveMember = true
            });
            await db.SaveChangesAsync();
        }

        
        [Fact]
        public async Task CreateAsync_ShouldDecrementAvailableCopies()
        {
            // Arrange
            using var db = CreateDb();
            await SeedBookAndCustomer(db);
            var svc = new BookLoanService(db);

            var loan = new BookLoan
            {
                BookId = 1,
                CustomerId = 1,
                LoanDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(14)
            };

            // Act
            await svc.CreateAsync(loan);

            // Assert
            var book = await db.Books.FindAsync(1);
            Assert.Equal(2, book!.AvailableCopies);
        }

        
        [Fact]
        public async Task CreateAsync_ShouldFail_WhenBookNotAvailable()
        {
            // Arrange
            using var db = CreateDb();
            await SeedBookAndCustomer(db);

            // make book unavailable
            var book = await db.Books.FindAsync(1);
            book!.AvailableCopies = 0;
            await db.SaveChangesAsync();

            var svc = new BookLoanService(db);

            var loan = new BookLoan
            {
                BookId = 1,
                CustomerId = 1,
                LoanDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(14)
            };

            // Act + Assert
            await Assert.ThrowsAsync<UnauthorizedLoanException>(
                () => svc.CreateAsync(loan));
        }

        
        [Fact]
        public async Task ReturnAsync_ShouldIncrementAvailableCopies()
        {
            // Arrange
            using var db = CreateDb();
            await SeedBookAndCustomer(db);
            var svc = new BookLoanService(db);

            var book = await db.Books.FindAsync(1);
            book!.AvailableCopies = 2;

            db.BookLoans.Add(new BookLoan
            {
                Id = 1,
                BookId = 1,
                CustomerId = 1,
                LoanDate = DateTime.UtcNow.AddDays(-10),
                DueDate = DateTime.UtcNow.AddDays(4),
                Status = LoanStatus.Active
            });
            await db.SaveChangesAsync();

            // Act
            var (ok, reason) = await svc.ReturnAsync(1, DateTime.UtcNow);

            // Assert
            Assert.True(ok);
            Assert.Null(reason);

            var updatedBook = await db.Books.FindAsync(1);
            Assert.Equal(3, updatedBook!.AvailableCopies);
        }

        // Test 14
        [Fact]
        public async Task MarkOverdueAsync_ShouldUpdateStatus_ForExpiredLoans()
        {
            // Arrange
            using var db = CreateDb();
            await SeedBookAndCustomer(db);

            db.BookLoans.Add(new BookLoan
            {
                Id = 1,
                BookId = 1,
                CustomerId = 1,
                LoanDate = DateTime.UtcNow.AddDays(-21),
                DueDate = DateTime.UtcNow.AddDays(-7),
                Status = LoanStatus.Active
            });
            await db.SaveChangesAsync();

            var svc = new BookLoanService(db);

            // Act
            var count = await svc.MarkOverdueAsync();

            // Assert
            Assert.Equal(1, count);

            var loan = await db.BookLoans.FindAsync(1);
            Assert.Equal(LoanStatus.Overdue, loan!.Status);
            Assert.True(loan.FineAmount > 0);
        }

        // Test 15
        [Fact]
        public async Task CreateAsync_ShouldFail_WhenCustomerHasOverdueLoans()
        {
            // Arrange
            using var db = CreateDb();
            await SeedBookAndCustomer(db);

            // Add another overdue loan for same customer
            db.Books.Add(new Book
            {
                Id = 2,
                Title = "Another Book",
                ISBN = "T-002",
                AuthorId = 1,
                AvailableCopies = 2,
                TotalCopies = 2
            });
            db.BookLoans.Add(new BookLoan
            {
                BookId = 2,
                CustomerId = 1,
                LoanDate = DateTime.UtcNow.AddDays(-30),
                DueDate = DateTime.UtcNow.AddDays(-10),
                Status = LoanStatus.Overdue
            });
            await db.SaveChangesAsync();

            var svc = new BookLoanService(db);

            var loan = new BookLoan
            {
                BookId = 1,
                CustomerId = 1,
                LoanDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(14)
            };

            // Act + Assert
            await Assert.ThrowsAsync<UnauthorizedLoanException>(
                () => svc.CreateAsync(loan));
        }
    }
}