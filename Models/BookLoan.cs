using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    public enum LoanStatus
    {
        Active,
        Returned,
        Overdue,
        Lost,
        Cancelled
    }

    public class BookLoan
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Book")]
        public int BookId { get; set; }

        [Required]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        [Display(Name = "Library Branch")]
        public int? LibraryBranchId { get; set; }

        [Required]
        [Display(Name = "Loan Date")]
        [DataType(DataType.Date)]
        public DateTime LoanDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Display(Name = "Return Date")]
        [DataType(DataType.Date)]
        public DateTime? ReturnDate { get; set; }

        [Display(Name = "Status")]
        public LoanStatus Status { get; set; } = LoanStatus.Active;

        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "Fine Amount")]
        public decimal FineAmount { get; set; } = 0;

        [Display(Name = "Is Fine Paid")]
        public bool IsFinePaid { get; set; } = false;

        [StringLength(500)]
        public string? Notes { get; set; }

        [Display(Name = "Renewed Count")]
        public int RenewedCount { get; set; } = 0;

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Updated Date")]
        public DateTime? UpdatedDate { get; set; }

        // Navigation Properties
        [ForeignKey("BookId")]
        public virtual Book? Book { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        [ForeignKey("LibraryBranchId")]
        public virtual LibraryBranch? LibraryBranch { get; set; }
    }
}