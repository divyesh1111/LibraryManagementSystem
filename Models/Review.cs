using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Book")]
        public int BookId { get; set; }

        [Required]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [StringLength(100)]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Review content is required")]
        [StringLength(2000, MinimumLength = 10)]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Is Approved")]
        public bool IsApproved { get; set; } = false;

        [Display(Name = "Helpful Votes")]
        public int HelpfulVotes { get; set; } = 0;

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Updated Date")]
        public DateTime? UpdatedDate { get; set; }

        // Navigation Properties
        [ForeignKey("BookId")]
        public virtual Book? Book { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }
    }
}