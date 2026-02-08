using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagementSystem.ViewModels
{
    public class CustomerViewModel
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; } = string.Empty;

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
        public string? City { get; set; }

        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }

        public string? Country { get; set; }

        [Display(Name = "Membership Date")]
        public DateTime MembershipDate { get; set; }

        [Display(Name = "Library Card Number")]
        public string? LibraryCardNumber { get; set; }

        [Display(Name = "Is Active Member")]
        public bool IsActiveMember { get; set; }

        [Display(Name = "Profile Image")]
        public string? ProfileImageUrl { get; set; }

        [Display(Name = "Preferred Branch")]
        public string? PreferredBranchName { get; set; }

        public int ActiveLoansCount { get; set; }
    }

    public class CustomerListViewModel
    {
        public IEnumerable<CustomerViewModel> Customers { get; set; } = new List<CustomerViewModel>();
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
        public bool? ActiveOnly { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }

    public class CustomerCreateViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(20)]
        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Membership Expiry")]
        [DataType(DataType.Date)]
        public DateTime? MembershipExpiry { get; set; }

        [Display(Name = "Profile Image URL")]
        public string? ProfileImageUrl { get; set; }

        [Display(Name = "Preferred Branch")]
        public int? PreferredBranchId { get; set; }

        [Display(Name = "Is Active Member")]
        public bool IsActiveMember { get; set; } = true;

        public SelectList? LibraryBranches { get; set; }
    }

    public class CustomerEditViewModel : CustomerCreateViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Library Card Number")]
        public string? LibraryCardNumber { get; set; }
    }

    public class CustomerDetailsViewModel : CustomerViewModel
    {
        public DateTime? DateOfBirth { get; set; }
        public DateTime? MembershipExpiry { get; set; }
        public int? PreferredBranchId { get; set; }
        public IEnumerable<BookLoanViewModel> RecentLoans { get; set; } = new List<BookLoanViewModel>();
        public IEnumerable<ReviewViewModel> Reviews { get; set; } = new List<ReviewViewModel>();
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}