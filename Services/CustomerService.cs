using LibraryManagementSystem.Api.Dtos;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly LibraryDbContext _db;
        public CustomerService(LibraryDbContext db) => _db = db;

        public async Task<PagedResult<Customer>> GetPagedAsync(string? searchTerm, bool? activeOnly, string? sortBy, int page, int pageSize)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var q = _db.Customers.AsNoTracking()
                .Include(c => c.PreferredBranch)
                .Include(c => c.BookLoans)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                q = q.Where(c => c.FirstName.Contains(searchTerm) ||
                                 c.LastName.Contains(searchTerm) ||
                                 c.Email.Contains(searchTerm) ||
                                 (c.LibraryCardNumber != null && c.LibraryCardNumber.Contains(searchTerm)));
            }

            if (activeOnly == true) q = q.Where(c => c.IsActiveMember);

            q = sortBy switch
            {
                "name_desc" => q.OrderByDescending(c => c.LastName).ThenByDescending(c => c.FirstName),
                "email" => q.OrderBy(c => c.Email),
                "newest" => q.OrderByDescending(c => c.MembershipDate),
                "oldest" => q.OrderBy(c => c.MembershipDate),
                _ => q.OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
            };

            var total = await q.CountAsync();
            var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<Customer> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
        }

        public Task<Customer?> GetByIdAsync(int id) =>
            _db.Customers.AsNoTracking()
                .Include(c => c.PreferredBranch)
                .Include(c => c.BookLoans).ThenInclude(l => l.Book)
                .Include(c => c.Reviews).ThenInclude(r => r.Book)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<Customer> CreateAsync(Customer customer)
        {
            if (await _db.Customers.AnyAsync(c => c.Email == customer.Email))
                throw new InvalidOperationException("Customer email already exists.");

            customer.CreatedDate = DateTime.UtcNow;
            customer.MembershipDate = DateTime.UtcNow;
            customer.LibraryCardNumber ??= $"LIB-{DateTime.Now:yyyy}-{await _db.Customers.CountAsync() + 1:D4}";

            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();
            return customer;
        }

        public async Task<bool> UpdateAsync(int id, Customer customer)
        {
            var existing = await _db.Customers.FirstOrDefaultAsync(c => c.Id == id);
            if (existing == null) return false;

            if (await _db.Customers.AnyAsync(c => c.Email == customer.Email && c.Id != id))
                throw new InvalidOperationException("Customer email already exists.");

            existing.FirstName = customer.FirstName;
            existing.LastName = customer.LastName;
            existing.Email = customer.Email;
            existing.PhoneNumber = customer.PhoneNumber;
            existing.Address = customer.Address;
            existing.City = customer.City;
            existing.PostalCode = customer.PostalCode;
            existing.Country = customer.Country;
            existing.DateOfBirth = customer.DateOfBirth;
            existing.MembershipExpiry = customer.MembershipExpiry;
            existing.IsActiveMember = customer.IsActiveMember;
            existing.ProfileImageUrl = customer.ProfileImageUrl;
            existing.PreferredBranchId = customer.PreferredBranchId;
            existing.UpdatedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<(bool ok, string? reason)> DeleteAsync(int id)
        {
            var customer = await _db.Customers.Include(c => c.BookLoans).FirstOrDefaultAsync(c => c.Id == id);
            if (customer == null) return (false, "Not found.");

            if (customer.BookLoans.Any(l => l.Status == LoanStatus.Active))
                return (false, "Cannot delete customer with active loans.");

            _db.Customers.Remove(customer);
            await _db.SaveChangesAsync();
            return (true, null);
        }
    }
}