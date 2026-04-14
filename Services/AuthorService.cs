using LibraryManagementSystem.Api.Dtos;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly LibraryDbContext _db;
        public AuthorService(LibraryDbContext db) => _db = db;

        public async Task<PagedResult<Author>> GetPagedAsync(string? searchTerm, string? sortBy, int page, int pageSize)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var q = _db.Authors.AsNoTracking().Include(a => a.Books).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                q = q.Where(a => a.FirstName.Contains(searchTerm) ||
                                 a.LastName.Contains(searchTerm) ||
                                 (a.Nationality != null && a.Nationality.Contains(searchTerm)));
            }

            q = sortBy switch
            {
                "name_desc" => q.OrderByDescending(a => a.LastName).ThenByDescending(a => a.FirstName),
                "books" => q.OrderByDescending(a => a.Books.Count),
                "newest" => q.OrderByDescending(a => a.CreatedDate),
                _ => q.OrderBy(a => a.LastName).ThenBy(a => a.FirstName)
            };

            var total = await q.CountAsync();
            var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<Author> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
        }

        public Task<Author?> GetByIdAsync(int id) =>
            _db.Authors.Include(a => a.Books).AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

        public async Task<Author> CreateAsync(Author author)
        {
            author.CreatedDate = DateTime.UtcNow;
            _db.Authors.Add(author);
            await _db.SaveChangesAsync();
            return author;
        }

        public async Task<bool> UpdateAsync(int id, Author author)
        {
            var existing = await _db.Authors.FirstOrDefaultAsync(a => a.Id == id);
            if (existing == null) return false;

            existing.FirstName = author.FirstName;
            existing.LastName = author.LastName;
            existing.DateOfBirth = author.DateOfBirth;
            existing.Nationality = author.Nationality;
            existing.Biography = author.Biography;
            existing.WebsiteUrl = author.WebsiteUrl;
            existing.Email = author.Email;
            existing.ImageUrl = author.ImageUrl;
            existing.IsActive = author.IsActive;
            existing.UpdatedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<(bool ok, string? reason)> DeleteAsync(int id)
        {
            var author = await _db.Authors.Include(a => a.Books).FirstOrDefaultAsync(a => a.Id == id);
            if (author == null) return (false, "Not found.");
            if (author.Books.Any()) return (false, "Cannot delete author with associated books.");

            _db.Authors.Remove(author);
            await _db.SaveChangesAsync();
            return (true, null);
        }
    }
}