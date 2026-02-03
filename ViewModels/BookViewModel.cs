using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagementSystem.ViewModels
{
    public class BookViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string ISBN { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Display(Name = "Publication Date")]
        public DateTime? PublicationDate { get; set; }

        public string? Publisher { get; set; }

        [Display(Name = "Page Count")]
        public int? PageCount { get; set; }

        public string? Genre { get; set; }

        [Display(Name = "Cover Image")]
        public string? CoverImageUrl { get; set; }

        [Display(Name = "Available Copies")]
        public int AvailableCopies { get; set; }

        [Display(Name = "Total Copies")]
        public int TotalCopies { get; set; }

        [Display(Name = "Is Available")]
        public bool IsAvailable => AvailableCopies > 0;

        [Display(Name = "Author")]
        public int AuthorId { get; set; }

        [Display(Name = "Author Name")]
        public string? AuthorName { get; set; }

        [Display(Name = "Library Branch")]
        public int? LibraryBranchId { get; set; }

        [Display(Name = "Branch Name")]
        public string? LibraryBranchName { get; set; }
    }

    public class BookCreateViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(300, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 300 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "ISBN is required")]
        [StringLength(20, ErrorMessage = "ISBN cannot exceed 20 characters")]
        public string ISBN { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Display(Name = "Publication Date")]
        [DataType(DataType.Date)]
        public DateTime? PublicationDate { get; set; }

        [StringLength(200)]
        public string? Publisher { get; set; }

        [Range(1, 10000, ErrorMessage = "Page count must be between 1 and 10000")]
        [Display(Name = "Page Count")]
        public int? PageCount { get; set; }

        [StringLength(50)]
        public string? Genre { get; set; }

        [Display(Name = "Cover Image URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? CoverImageUrl { get; set; }

        [Range(0, 1000, ErrorMessage = "Available copies must be between 0 and 1000")]
        [Display(Name = "Available Copies")]
        public int AvailableCopies { get; set; } = 1;

        [Range(0, 1000, ErrorMessage = "Total copies must be between 0 and 1000")]
        [Display(Name = "Total Copies")]
        public int TotalCopies { get; set; } = 1;

        [Required(ErrorMessage = "Author is required")]
        [Display(Name = "Author")]
        public int AuthorId { get; set; }

        [Display(Name = "Library Branch")]
        public int? LibraryBranchId { get; set; }

        // For dropdown lists
        public SelectList? Authors { get; set; }
        public SelectList? LibraryBranches { get; set; }
    }

    public class BookEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(300, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 300 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "ISBN is required")]
        [StringLength(20, ErrorMessage = "ISBN cannot exceed 20 characters")]
        public string ISBN { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Display(Name = "Publication Date")]
        [DataType(DataType.Date)]
        public DateTime? PublicationDate { get; set; }

        [StringLength(200)]
        public string? Publisher { get; set; }

        [Range(1, 10000, ErrorMessage = "Page count must be between 1 and 10000")]
        [Display(Name = "Page Count")]
        public int? PageCount { get; set; }

        [StringLength(50)]
        public string? Genre { get; set; }

        [Display(Name = "Cover Image URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? CoverImageUrl { get; set; }

        [Range(0, 1000, ErrorMessage = "Available copies must be between 0 and 1000")]
        [Display(Name = "Available Copies")]
        public int AvailableCopies { get; set; } = 1;

        [Range(0, 1000, ErrorMessage = "Total copies must be between 0 and 1000")]
        [Display(Name = "Total Copies")]
        public int TotalCopies { get; set; } = 1;

        [Required(ErrorMessage = "Author is required")]
        [Display(Name = "Author")]
        public int AuthorId { get; set; }

        [Display(Name = "Library Branch")]
        public int? LibraryBranchId { get; set; }

        // For dropdown lists
        public SelectList? Authors { get; set; }
        public SelectList? LibraryBranches { get; set; }
    }

    public class BookDetailsViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string ISBN { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Display(Name = "Publication Date")]
        public DateTime? PublicationDate { get; set; }

        public string? Publisher { get; set; }

        [Display(Name = "Page Count")]
        public int? PageCount { get; set; }

        public string? Genre { get; set; }

        [Display(Name = "Cover Image")]
        public string? CoverImageUrl { get; set; }

        [Display(Name = "Available Copies")]
        public int AvailableCopies { get; set; }

        [Display(Name = "Total Copies")]
        public int TotalCopies { get; set; }

        [Display(Name = "Is Available")]
        public bool IsAvailable => AvailableCopies > 0;

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Author")]
        public int AuthorId { get; set; }

        [Display(Name = "Author Name")]
        public string? AuthorName { get; set; }

        [Display(Name = "Library Branch")]
        public int? LibraryBranchId { get; set; }

        [Display(Name = "Branch Name")]
        public string? LibraryBranchName { get; set; }
    }
}