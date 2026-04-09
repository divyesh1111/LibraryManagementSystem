using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services
{
    public class BranchService : IBranchService
    {
        private readonly LibraryDbContext _context;

        public BranchService(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LibraryBranch>> GetAllBranchesAsync()
        {
            return await _context.LibraryBranches
                .Include(b => b.Books)
                .Include(b => b.Customers)
                .OrderBy(b => b.BranchName)
                .ToListAsync();
        }

        public async Task<LibraryBranch?> GetBranchByIdAsync(int id)
        {
            return await _context.LibraryBranches.FindAsync(id);
        }

        public async Task<LibraryBranch?> GetBranchWithDetailsAsync(int id)
        {
            return await _context.LibraryBranches
                .Include(b => b.Books)
                    .ThenInclude(book => book.Author)
                .Include(b => b.Customers)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<LibraryBranch> CreateBranchAsync(LibraryBranch branch)
        {
            branch.CreatedDate = DateTime.UtcNow;
            _context.LibraryBranches.Add(branch);
            await _context.SaveChangesAsync();
            return branch;
        }

        public async Task<LibraryBranch?> UpdateBranchAsync(int id, LibraryBranch updatedBranch)
        {
            var branch = await _context.LibraryBranches.FindAsync(id);
            if (branch == null) return null;

            branch.BranchName = updatedBranch.BranchName;
            branch.Address = updatedBranch.Address;
            branch.City = updatedBranch.City;
            branch.State = updatedBranch.State;
            branch.PostalCode = updatedBranch.PostalCode;
            branch.Country = updatedBranch.Country;
            branch.PhoneNumber = updatedBranch.PhoneNumber;
            branch.Email = updatedBranch.Email;
            branch.Website = updatedBranch.Website;
            branch.OpeningHours = updatedBranch.OpeningHours;
            branch.ManagerName = updatedBranch.ManagerName;
            branch.EstablishedDate = updatedBranch.EstablishedDate;
            branch.IsActive = updatedBranch.IsActive;
            branch.ImageUrl = updatedBranch.ImageUrl;
            branch.TotalCapacity = updatedBranch.TotalCapacity;
            branch.Description = updatedBranch.Description;
            branch.Latitude = updatedBranch.Latitude;
            branch.Longitude = updatedBranch.Longitude;
            branch.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return branch;
        }

        public async Task<bool> DeleteBranchAsync(int id)
        {
            var branch = await _context.LibraryBranches
                .Include(b => b.Books)
                .Include(b => b.BookLoans)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (branch == null) return false;
            if (await BranchHasBooksOrLoansAsync(id)) return false;

            _context.LibraryBranches.Remove(branch);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> BranchHasBooksOrLoansAsync(int id)
        {
            var hasBooks = await _context.Books.AnyAsync(b => b.LibraryBranchId == id);
            var hasActiveLoans = await _context.BookLoans
                .AnyAsync(l => l.LibraryBranchId == id && l.Status == LoanStatus.Active);

            return hasBooks || hasActiveLoans;
        }
    }
}