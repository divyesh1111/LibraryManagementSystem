using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly LibraryDbContext _context;

        public CustomerService(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers
                .Include(c => c.PreferredBranch)
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .ToListAsync();
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers
                .Include(c => c.PreferredBranch)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            customer.MembershipDate = DateTime.UtcNow;
            customer.CreatedDate = DateTime.UtcNow;
            customer.LibraryCardNumber = $"LIB-{DateTime.UtcNow:yyyy}-{await _context.Customers.CountAsync() + 1:D4}";

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer?> UpdateCustomerAsync(int id, Customer updatedCustomer)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return null;

            customer.FirstName = updatedCustomer.FirstName;
            customer.LastName = updatedCustomer.LastName;
            customer.Email = updatedCustomer.Email;
            customer.PhoneNumber = updatedCustomer.PhoneNumber;
            customer.Address = updatedCustomer.Address;
            customer.City = updatedCustomer.City;
            customer.PostalCode = updatedCustomer.PostalCode;
            customer.Country = updatedCustomer.Country;
            customer.DateOfBirth = updatedCustomer.DateOfBirth;
            customer.MembershipExpiry = updatedCustomer.MembershipExpiry;
            customer.IsActiveMember = updatedCustomer.IsActiveMember;
            customer.ProfileImageUrl = updatedCustomer.ProfileImageUrl;
            customer.PreferredBranchId = updatedCustomer.PreferredBranchId;
            customer.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return false;

            if (await CustomerHasActiveLoansAsync(id)) return false;

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            if (excludeId.HasValue)
                return await _context.Customers.AnyAsync(c => c.Email == email && c.Id != excludeId.Value);

            return await _context.Customers.AnyAsync(c => c.Email == email);
        }

        public async Task<bool> CustomerHasActiveLoansAsync(int id)
        {
            return await _context.BookLoans
                .AnyAsync(l => l.CustomerId == id && l.Status == LoanStatus.Active);
        }
    }
}