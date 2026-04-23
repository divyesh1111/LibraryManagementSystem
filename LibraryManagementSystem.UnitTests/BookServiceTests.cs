using LibraryManagementSystem.Data;
using LibraryManagementSystem.Exceptions;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibraryManagementSystem.UnitTests
{
    public class BookServiceTests
    {
        private static LibraryDbContext CreateDb()
        {
            var opts = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
                .Options;
            return new LibraryDbContext(opts);
        }

        private static void SeedAuthor(LibraryDbContext db)
        {
            db.Authors.Add(new Author { Id = 1, FirstName = "Test", LastName = "Author" });
            db.SaveChanges();
        }

        // Test 6
        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenDuplicateIsbn()
        {
            // Arrange
            using var db = CreateDb();
            SeedAuthor(db);
            var svc = new BookService(db);

            db.Books.Add(new Book
            {
                Title = "Existing Book",
                ISBN = "DUPLICATE-001",
                AuthorId = 1,
                AvailableCopies = 1,
                TotalCopies = 1
            });
            await db.SaveChangesAsync();

            var newBook = new Book
            {
                Title = "New Book",
                ISBN = "DUPLICATE-001",
                AuthorId = 1,
                AvailableCopies = 1,
                TotalCopies = 1
            };

            // Act + Assert
            await Assert.ThrowsAsync<DuplicateIsbnException>(
                () => svc.CreateAsync(newBook));
        }

        // Test 7
        [Fact]
        public async Task CreateAsync_ShouldSucceed_WithUniqueIsbn()
        {
            // Arrange
            using var db = CreateDb();
            SeedAuthor(db);
            var svc = new BookService(db);

            var book = new Book
            {
                Title = "Unique Book",
                ISBN = "UNIQUE-001",
                AuthorId = 1,
                AvailableCopies = 2,
                TotalCopies = 2
            };

            // Act
            var result = await svc.CreateAsync(book);

            // Assert
            Assert.True(result.Id > 0);
            Assert.Equal("UNIQUE-001", result.ISBN);
            Assert.Equal(1, await db.Books.CountAsync());
        }

        // Test 8
        [Fact]
        public async Task DeleteAsync_ShouldFail_WhenBookHasActiveLoans()
        {
            // Arrange
            using var db = CreateDb();
            SeedAuthor(db);
            var svc = new BookService(db);

            db.Books.Add(new Book
            {
                Id = 1,
                Title = "Borrowed Book",
                ISBN = "LOAN-001",
                AuthorId = 1,
                AvailableCopies = 0,
                TotalCopies = 1
            });
            db.Customers.Add(new Customer
            {
                Id = 1,
                FirstName = "Test",
                LastName = "Customer",
                Email = "test@test.com"
            });
            db.BookLoans.Add(new BookLoan
            {
                BookId = 1,
                CustomerId = 1,
                LoanDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(7),
                Status = LoanStatus.Active
            });
            await db.SaveChangesAsync();

            // Act
            var (ok, reason) = await svc.DeleteAsync(1);

            // Assert
            Assert.False(ok);
            Assert.NotNull(reason);
            Assert.Contains("loan", reason!.ToLower());
        }

        // Test 9
        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenBookDoesNotExist()
        {
            // Arrange
            using var db = CreateDb();
            var svc = new BookService(db);

            // Act
            var result = await svc.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        // Test 10
        [Fact]
        public async Task GetPagedAsync_ShouldFilterBySearchTerm()
        {
            // Arrange
            using var db = CreateDb();
            SeedAuthor(db);
            var svc = new BookService(db);

            db.Books.AddRange(
                new Book { Title = "Wings of Fire", ISBN = "B001", AuthorId = 1, AvailableCopies = 1, TotalCopies = 1 },
                new Book { Title = "The White Tiger", ISBN = "B002", AuthorId = 1, AvailableCopies = 1, TotalCopies = 1 },
                new Book { Title = "Gitanjali", ISBN = "B003", AuthorId = 1, AvailableCopies = 1, TotalCopies = 1 }
            );
            await db.SaveChangesAsync();

            // Act
            var result = await svc.GetPagedAsync("Wings", null, null, null, null, null, 1, 10);

            // Assert
            Assert.Equal(1, result.TotalCount);
            Assert.Equal("Wings of Fire", result.Items[0].Title);
        }
    }
}