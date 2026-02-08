using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class LibraryBranch
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Branch name is required")]
        [StringLength(200, MinimumLength = 2)]
        [Display(Name = "Branch Name")]
        public string BranchName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [StringLength(20)]
        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Url]
        [Display(Name = "Website")]
        public string? Website { get; set; }

        [Display(Name = "Opening Hours")]
        [StringLength(200)]
        public string? OpeningHours { get; set; }

        [Display(Name = "Manager Name")]
        [StringLength(200)]
        public string? ManagerName { get; set; }

        [Display(Name = "Established Date")]
        [DataType(DataType.Date)]
        public DateTime? EstablishedDate { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Total Capacity")]
        public int? TotalCapacity { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Display(Name = "Latitude")]
        public double? Latitude { get; set; }

        [Display(Name = "Longitude")]
        public double? Longitude { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Updated Date")]
        public DateTime? UpdatedDate { get; set; }

        // Navigation Properties
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
        public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
        public virtual ICollection<BookLoan> BookLoans { get; set; } = new List<BookLoan>();
    }
}