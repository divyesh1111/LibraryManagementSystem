using System.ComponentModel.DataAnnotations;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagementSystem.ViewModels
{
    public class BookViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(300, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "ISBN is required")]
        [StringLength(20)]
        public string ISBN { get; set; } = string.Empty;

        [StringLength(3000)]
        public string? Description { get; set; }

        [Display(Name = "Publication Date")]
        [DataType(DataType.Date)]
        public DateTime? PublicationDate { get; set; }

        [StringLength(200)]
        public string? Publisher { get; set; }

        [Display(Name = "Page Count")]
        public int? PageCount { get; set; }

        public string? Language { get; set; }

        [Display(Name = "Cover Image")]
        public string? CoverImageUrl { get; set; }

        public decimal? Price { get; set; }

        [Display(Name = "Available Copies")]
        public int AvailableCopies { get; set; }

        [Display(Name = "Total Copies")]
        public int TotalCopies { get; set; }

        public bool IsAvailable => AvailableCopies > 0;

        // Related data
        public int AuthorId { get; set; }
        public string? AuthorName { get; set; }

        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public int? LibraryBranchId { get; set; }
        public string? LibraryBranchName { get; set; }

        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }

    public class BookListViewModel
    {
        public IEnumerable<BookViewModel> Books { get; set; } = new List<BookViewModel>();
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
        public int? CategoryFilter { get; set; }
        public int? AuthorFilter { get; set; }
        public int? BranchFilter { get; set; }
        public bool? AvailableOnly { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        
        public SelectList? Categories { get; set; }
        public SelectList? Authors { get; set; }
        public SelectList? Branches { get; set; }
    }

    public class BookCreateViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(300, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "ISBN is required")]
        [StringLength(20)]
        public string ISBN { get; set; } = string.Empty;

        [StringLength(3000)]
        public string? Description { get; set; }

        [Display(Name = "Publication Date")]
        [DataType(DataType.Date)]
        public DateTime? PublicationDate { get; set; }

        [StringLength(200)]
        public string? Publisher { get; set; }

        [Range(1, 10000)]
        [Display(Name = "Page Count")]
        public int? PageCount { get; set; }

        public string? Language { get; set; } = "English";

        [Display(Name = "Cover Image URL")]
        public string? CoverImageUrl { get; set; }

        [Range(0, 10000)]
        public decimal? Price { get; set; }

        [Range(0, 10000)]
        [Display(Name = "Available Copies")]
        public int AvailableCopies { get; set; } = 1;

        [Range(0, 10000)]
        [Display(Name = "Total Copies")]
        public int TotalCopies { get; set; } = 1;

        [Required(ErrorMessage = "Author is required")]
        [Display(Name = "Author")]
        public int AuthorId { get; set; }

        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        [Display(Name = "Library Branch")]
        public int? LibraryBranchId { get; set; }

        // Dropdowns
        public SelectList? Authors { get; set; }
        public SelectList? Categories { get; set; }
        public SelectList? LibraryBranches { get; set; }
    }

    public class BookEditViewModel : BookCreateViewModel
    {
        public int Id { get; set; }
    }

    public class BookDetailsViewModel : BookViewModel
    {
        public Author? Author { get; set; }
        public Category? Category { get; set; }
        public LibraryBranch? LibraryBranch { get; set; }
        public IEnumerable<Review>? Reviews { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}