using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";

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

        [Display(Name = "Membership Date")]
        [DataType(DataType.Date)]
        public DateTime MembershipDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Membership Expiry")]
        [DataType(DataType.Date)]
        public DateTime? MembershipExpiry { get; set; }

        [Display(Name = "Library Card Number")]
        [StringLength(50)]
        public string? LibraryCardNumber { get; set; }

        [Display(Name = "Is Active Member")]
        public bool IsActiveMember { get; set; } = true;

        [Display(Name = "Profile Image URL")]
        public string? ProfileImageUrl { get; set; }

        [Display(Name = "Preferred Branch")]
        public int? PreferredBranchId { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Updated Date")]
        public DateTime? UpdatedDate { get; set; }

        // Navigation Properties
        [ForeignKey("PreferredBranchId")]
        public virtual LibraryBranch? PreferredBranch { get; set; }

        public virtual ICollection<BookLoan> BookLoans { get; set; } = new List<BookLoan>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}