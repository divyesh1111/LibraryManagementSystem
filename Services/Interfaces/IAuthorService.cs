using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services.Interfaces
{
    public interface IAuthorService
    {
        Task<IEnumerable<Author>> GetAllAuthorsAsync();
        Task<Author?> GetAuthorByIdAsync(int id);
        Task<Author?> GetAuthorWithBooksAsync(int id);
        Task<Author> CreateAuthorAsync(Author author);
        Task<Author?> UpdateAuthorAsync(int id, Author author);
        Task<bool> DeleteAuthorAsync(int id);
        Task<bool> AuthorHasBooksAsync(int id);
    }
}