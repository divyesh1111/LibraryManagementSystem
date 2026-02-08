using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.ViewModels
{
    public class LibraryBranchViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Branch Name")]
        public string BranchName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? State { get; set; }

        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }

        public string? Country { get; set; }

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        [Display(Name = "Opening Hours")]
        public string? OpeningHours { get; set; }

        [Display(Name = "Manager Name")]
        public string? ManagerName { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Image")]
        public string? ImageUrl { get; set; }

        public int BookCount { get; set; }
        public int CustomerCount { get; set; }
        public int ActiveLoansCount { get; set; }
    }

    public class LibraryBranchListViewModel
    {
        public IEnumerable<LibraryBranchViewModel> Branches { get; set; } = new List<LibraryBranchViewModel>();
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
        public bool? ActiveOnly { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }

    public class LibraryBranchCreateViewModel
    {
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
        public string? Email { get; set; }

        [Url]
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

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

    public class LibraryBranchEditViewModel : LibraryBranchCreateViewModel
    {
        public int Id { get; set; }
    }

    public class LibraryBranchDetailsViewModel : LibraryBranchViewModel
    {
        public string? Website { get; set; }
        public DateTime? EstablishedDate { get; set; }
        public int? TotalCapacity { get; set; }
        public string? Description { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public IEnumerable<BookViewModel> Books { get; set; } = new List<BookViewModel>();
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}