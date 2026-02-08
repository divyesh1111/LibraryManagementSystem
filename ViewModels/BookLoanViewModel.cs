using System.ComponentModel.DataAnnotations;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagementSystem.ViewModels
{
    public class BookLoanViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Book")]
        public string BookTitle { get; set; } = string.Empty;

        [Display(Name = "Customer")]
        public string CustomerName { get; set; } = string.Empty;

        [Display(Name = "Branch")]
        public string? BranchName { get; set; }

        [Display(Name = "Loan Date")]
        public DateTime LoanDate { get; set; }

        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [Display(Name = "Return Date")]
        public DateTime? ReturnDate { get; set; }

        public LoanStatus Status { get; set; }

        [Display(Name = "Fine Amount")]
        public decimal FineAmount { get; set; }

        [Display(Name = "Fine Paid")]
        public bool IsFinePaid { get; set; }

        public int BookId { get; set; }
        public int CustomerId { get; set; }
        public int? LibraryBranchId { get; set; }

        public bool IsOverdue => Status == LoanStatus.Active && DateTime.UtcNow > DueDate;
        public int DaysOverdue => IsOverdue ? (int)(DateTime.UtcNow - DueDate).TotalDays : 0;
    }

    public class BookLoanListViewModel
    {
        public IEnumerable<BookLoanViewModel> Loans { get; set; } = new List<BookLoanViewModel>();
        public string? SearchTerm { get; set; }
        public LoanStatus? StatusFilter { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }

    public class BookLoanCreateViewModel
    {
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
        public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(14);

        [StringLength(500)]
        public string? Notes { get; set; }

        public SelectList? Books { get; set; }
        public SelectList? Customers { get; set; }
        public SelectList? LibraryBranches { get; set; }
    }

    public class BookLoanReturnViewModel
    {
        public int Id { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }

        [Required]
        [Display(Name = "Return Date")]
        [DataType(DataType.Date)]
        public DateTime ReturnDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Fine Amount")]
        public decimal FineAmount { get; set; }

        public string? Notes { get; set; }
    }
}