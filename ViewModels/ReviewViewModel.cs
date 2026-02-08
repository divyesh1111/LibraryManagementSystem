using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagementSystem.ViewModels
{
    public class ReviewViewModel
    {
        public int Id { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Title { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public int HelpfulVotes { get; set; }
        public DateTime CreatedDate { get; set; }
        public int BookId { get; set; }
        public int CustomerId { get; set; }
    }

    public class ReviewCreateViewModel
    {
        [Required]
        public int BookId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [StringLength(100)]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Review content is required")]
        [StringLength(2000, MinimumLength = 10)]
        public string Content { get; set; } = string.Empty;

        public SelectList? Books { get; set; }
        public SelectList? Customers { get; set; }
    }
}