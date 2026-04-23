using LibraryManagementSystem.Api.Dtos;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services
{
    public class LibraryBranchService : ILibraryBranchService
    {
        private readonly LibraryDbContext _db;
        public LibraryBranchService(LibraryDbContext db) => _db = db;

        public async Task<PagedResult<LibraryBranch>> GetPagedAsync(string? searchTerm, bool? activeOnly, string? sortBy, int page, int pageSize)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var q = _db.LibraryBranches.AsNoTracking()
                .Include(b => b.Books)
                .Include(b => b.Customers)
                .Include(b => b.BookLoans)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                q = q.Where(b => b.BranchName.Contains(searchTerm) ||
                                 (b.City != null && b.City.Contains(searchTerm)) ||
                                 b.Address.Contains(searchTerm));

            if (activeOnly == true) q = q.Where(b => b.IsActive);

            q = sortBy switch
            {
                "name_desc" => q.OrderByDescending(b => b.BranchName),
                "city" => q.OrderBy(b => b.City),
                "books" => q.OrderByDescending(b => b.Books.Count),
                "newest" => q.OrderByDescending(b => b.EstablishedDate),
                _ => q.OrderBy(b => b.BranchName)
            };

            var total = await q.CountAsync();
            var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<LibraryBranch> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
        }

        public Task<LibraryBranch?> GetByIdAsync(int id) =>
            _db.LibraryBranches.AsNoTracking()
                .Include(b => b.Books).ThenInclude(x => x.Author)
                .Include(b => b.Customers)
                .Include(b => b.BookLoans)
                .FirstOrDefaultAsync(b => b.Id == id);

        public async Task<LibraryBranch> CreateAsync(LibraryBranch branch)
        {
            branch.CreatedDate = DateTime.UtcNow;
            _db.LibraryBranches.Add(branch);
            await _db.SaveChangesAsync();
            return branch;
        }

        public async Task<bool> UpdateAsync(int id, LibraryBranch branch)
        {
            var existing = await _db.LibraryBranches.FirstOrDefaultAsync(b => b.Id == id);
            if (existing == null) return false;

            existing.BranchName = branch.BranchName;
            existing.Address = branch.Address;
            existing.City = branch.City;
            existing.State = branch.State;
            existing.PostalCode = branch.PostalCode;
            existing.Country = branch.Country;
            existing.PhoneNumber = branch.PhoneNumber;
            existing.Email = branch.Email;
            existing.Website = branch.Website;
            existing.OpeningHours = branch.OpeningHours;
            existing.ManagerName = branch.ManagerName;
            existing.EstablishedDate = branch.EstablishedDate;
            existing.IsActive = branch.IsActive;
            existing.ImageUrl = branch.ImageUrl;
            existing.TotalCapacity = branch.TotalCapacity;
            existing.Description = branch.Description;
            existing.Latitude = branch.Latitude;
            existing.Longitude = branch.Longitude;
            existing.UpdatedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<(bool ok, string? reason)> DeleteAsync(int id)
        {
            var branch = await _db.LibraryBranches
                .Include(b => b.Books)
                .Include(b => b.BookLoans)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (branch == null) return (false, "Not found.");

            if (branch.Books.Any() || branch.BookLoans.Any(l => l.Status == LoanStatus.Active))
                return (false, "Cannot delete branch with books or active loans.");

            _db.LibraryBranches.Remove(branch);
            await _db.SaveChangesAsync();
            return (true, null);
        }
    }
}