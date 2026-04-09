using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly LibraryDbContext _context;

        public AuthorService(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
        {
            return await _context.Authors
                .Include(a => a.Books)
                .OrderBy(a => a.LastName)
                .ThenBy(a => a.FirstName)
                .ToListAsync();
        }

        public async Task<Author?> GetAuthorByIdAsync(int id)
        {
            return await _context.Authors.FindAsync(id);
        }

        public async Task<Author?> GetAuthorWithBooksAsync(int id)
        {
            return await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Author> CreateAuthorAsync(Author author)
        {
            author.CreatedDate = DateTime.UtcNow;
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
            return author;
        }

        public async Task<Author?> UpdateAuthorAsync(int id, Author updatedAuthor)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return null;

            author.FirstName = updatedAuthor.FirstName;
            author.LastName = updatedAuthor.LastName;
            author.DateOfBirth = updatedAuthor.DateOfBirth;
            author.Nationality = updatedAuthor.Nationality;
            author.Biography = updatedAuthor.Biography;
            author.WebsiteUrl = updatedAuthor.WebsiteUrl;
            author.Email = updatedAuthor.Email;
            author.ImageUrl = updatedAuthor.ImageUrl;
            author.IsActive = updatedAuthor.IsActive;
            author.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return author;
        }

        public async Task<bool> DeleteAuthorAsync(int id)
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null) return false;
            if (author.Books.Any()) return false;

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AuthorHasBooksAsync(int id)
        {
            return await _context.Books.AnyAsync(b => b.AuthorId == id);
        }
    }
}