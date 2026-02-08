using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.ViewModels
{
    public class AuthorViewModel
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

        public string FullName => $"{FirstName} {LastName}";

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(100)]
        public string? Nationality { get; set; }

        [StringLength(2000)]
        public string? Biography { get; set; }

        [Url]
        [Display(Name = "Website")]
        public string? WebsiteUrl { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        public int BookCount { get; set; }
    }

    public class AuthorListViewModel
    {
        public IEnumerable<AuthorViewModel> Authors { get; set; } = new List<AuthorViewModel>();
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }

    public class AuthorCreateViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(100)]
        public string? Nationality { get; set; }

        [StringLength(2000)]
        public string? Biography { get; set; }

        [Url]
        [Display(Name = "Website")]
        public string? WebsiteUrl { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
    }

    public class AuthorEditViewModel : AuthorCreateViewModel
    {
        public int Id { get; set; }
    }

    public class AuthorDetailsViewModel : AuthorViewModel
    {
        public IEnumerable<BookViewModel> Books { get; set; } = new List<BookViewModel>();
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}