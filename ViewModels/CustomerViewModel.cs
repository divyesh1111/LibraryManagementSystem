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

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; } = string.Empty;

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        [Display(Name = "Membership Date")]
        public DateTime MembershipDate { get; set; }

        [Display(Name = "Library Card Number")]
        public string? LibraryCardNumber { get; set; }

        [Display(Name = "Is Active Member")]
        public bool IsActiveMember { get; set; }

        [Display(Name = "Preferred Branch")]
        public string? PreferredBranchName { get; set; }
    }

    public class CustomerCreateViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 100 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 100 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [StringLength(300)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [Display(Name = "Is Active Member")]
        public bool IsActiveMember { get; set; } = true;

        [Display(Name = "Preferred Branch")]
        public int? PreferredBranchId { get; set; }

        // For dropdown list
        public SelectList? LibraryBranches { get; set; }
    }

    public class CustomerEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 100 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 100 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [StringLength(300)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [Display(Name = "Library Card Number")]
        public string? LibraryCardNumber { get; set; }

        [Display(Name = "Is Active Member")]
        public bool IsActiveMember { get; set; } = true;

        [Display(Name = "Preferred Branch")]
        public int? PreferredBranchId { get; set; }

        // For dropdown list
        public SelectList? LibraryBranches { get; set; }
    }

    public class CustomerDetailsViewModel
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; } = string.Empty;

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        [Display(Name = "Membership Date")]
        public DateTime MembershipDate { get; set; }

        [Display(Name = "Library Card Number")]
        public string? LibraryCardNumber { get; set; }

        [Display(Name = "Is Active Member")]
        public bool IsActiveMember { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Preferred Branch")]
        public int? PreferredBranchId { get; set; }

        [Display(Name = "Preferred Branch")]
        public string? PreferredBranchName { get; set; }
    }
}