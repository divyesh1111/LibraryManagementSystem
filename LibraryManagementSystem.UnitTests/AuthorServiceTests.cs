using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibraryManagementSystem.UnitTests
{
    public class AuthorServiceTests
    {
        private static LibraryDbContext CreateDb()
        {
            var opts = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
                .Options;
            return new LibraryDbContext(opts);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddAuthor_AndReturnWithId()
        {
            using var db = CreateDb();
            var svc = new AuthorService(db);
            var author = new Author
            {
                FirstName = "Rabindranath",
                LastName = "Tagore",
                Nationality = "Indian"
            };

            var result = await svc.CreateAsync(author);

            Assert.True(result.Id > 0);
            Assert.Equal("Rabindranath", result.FirstName);
            Assert.Equal(1, await db.Authors.CountAsync());
        }

        [Fact]
        public async Task DeleteAsync_ShouldFail_WhenAuthorHasBooks()
        {
            using var db = CreateDb();
            var svc = new AuthorService(db);

            db.Authors.Add(new Author { Id = 1, FirstName = "A", LastName = "B" });
            db.Books.Add(new Book
            {
                Id = 1,
                Title = "Some Book",
                ISBN = "ISBN-001",
                AuthorId = 1,
                AvailableCopies = 1,
                TotalCopies = 1
            });
            await db.SaveChangesAsync();

            var (ok, reason) = await svc.DeleteAsync(1);

            Assert.False(ok);
            Assert.NotNull(reason);
            Assert.Contains("books", reason!.ToLower());
        }

        [Fact]
        public async Task DeleteAsync_ShouldSucceed_WhenAuthorHasNoBooks()
        {
            // Arrange
            using var db = CreateDb();
            var svc = new AuthorService(db);

            db.Authors.Add(new Author { Id = 1, FirstName = "A", LastName = "B" });
            await db.SaveChangesAsync();

            var (ok, reason) = await svc.DeleteAsync(1);

            Assert.True(ok);
            Assert.Null(reason);
            Assert.Equal(0, await db.Authors.CountAsync());
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenAuthorDoesNotExist()
        {
            using var db = CreateDb();
            var svc = new AuthorService(db);

            var author = new Author
            {
                FirstName = "Ghost",
                LastName = "Author"
            };

            var result = await svc.UpdateAsync(999, author);

            Assert.False(result);
        }

        [Fact]
        public async Task GetPagedAsync_ShouldReturnCorrectPage()
        {
            using var db = CreateDb();
            var svc = new AuthorService(db);

            for (int i = 1; i <= 15; i++)
            {
                db.Authors.Add(new Author
                {
                    FirstName = $"Author{i}",
                    LastName = $"Last{i}"
                });
            }
            await db.SaveChangesAsync();

            var result = await svc.GetPagedAsync(null, null, 2, 5);

            Assert.Equal(15, result.TotalCount);
            Assert.Equal(5, result.Items.Count);
            Assert.Equal(2, result.Page);
            Assert.Equal(3, result.TotalPages);
        }
    }
}