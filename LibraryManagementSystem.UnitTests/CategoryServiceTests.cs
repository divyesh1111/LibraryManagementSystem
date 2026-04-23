using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibraryManagementSystem.UnitTests
{
    public class CategoryServiceTests
    {
        private static LibraryDbContext CreateDb()
        {
            var opts = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
                .Options;
            return new LibraryDbContext(opts);
        }

        // Test 16 (already 15 above, this adds 1 more for safety)
        [Fact]
        public async Task CreateAsync_ShouldAddCategory_WithCreatedDate()
        {
            // Arrange
            using var db = CreateDb();
            var svc = new CategoryService(db);

            var category = new Category
            {
                Name = "Fiction",
                Description = "Fictional books"
            };

            // Act
            var result = await svc.CreateAsync(category);

            // Assert
            Assert.True(result.Id > 0);
            Assert.Equal("Fiction", result.Name);
            Assert.True(result.CreatedDate <= DateTime.UtcNow);
            Assert.Equal(1, await db.Categories.CountAsync());
        }

        [Fact]
        public async Task DeleteAsync_ShouldFail_WhenCategoryHasBooks()
        {
            // Arrange
            using var db = CreateDb();
            var svc = new CategoryService(db);

            db.Authors.Add(new Author { Id = 1, FirstName = "A", LastName = "B" });
            db.Categories.Add(new Category { Id = 1, Name = "Fiction" });
            db.Books.Add(new Book
            {
                Title = "Some Book",
                ISBN = "CAT-001",
                AuthorId = 1,
                CategoryId = 1,
                AvailableCopies = 1,
                TotalCopies = 1
            });
            await db.SaveChangesAsync();

            // Act
            var (ok, reason) = await svc.DeleteAsync(1);

            // Assert
            Assert.False(ok);
            Assert.NotNull(reason);
            Assert.Contains("books", reason!.ToLower());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenCategoryDoesNotExist()
        {
            // Arrange
            using var db = CreateDb();
            var svc = new CategoryService(db);

            // Act
            var result = await svc.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }
    }
}