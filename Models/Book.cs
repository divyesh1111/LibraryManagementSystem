using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(300, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "ISBN is required")]
        [StringLength(20)]
        [Display(Name = "ISBN")]
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

        [StringLength(50)]
        public string? Language { get; set; } = "English";

        [Display(Name = "Cover Image URL")]
        public string? CoverImageUrl { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        [Range(0, 10000)]
        public decimal? Price { get; set; }

        [Range(0, 10000)]
        [Display(Name = "Available Copies")]
        public int AvailableCopies { get; set; } = 1;

        [Range(0, 10000)]
        [Display(Name = "Total Copies")]
        public int TotalCopies { get; set; } = 1;

        [Display(Name = "Is Available")]
        public bool IsAvailable => AvailableCopies > 0;

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Updated Date")]
        public DateTime? UpdatedDate { get; set; }

        // Foreign Keys
        [Required]
        [Display(Name = "Author")]
        public int AuthorId { get; set; }

        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        [Display(Name = "Library Branch")]
        public int? LibraryBranchId { get; set; }

        // Navigation Properties
        [ForeignKey("AuthorId")]
        public virtual Author? Author { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        [ForeignKey("LibraryBranchId")]
        public virtual LibraryBranch? LibraryBranch { get; set; }

        public virtual ICollection<BookLoan> BookLoans { get; set; } = new List<BookLoan>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}