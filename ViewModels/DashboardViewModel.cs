namespace LibraryManagementSystem.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalBooks { get; set; }
        public int TotalAuthors { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalBranches { get; set; }
        public int TotalCategories { get; set; }
        public int ActiveLoans { get; set; }
        public int OverdueLoans { get; set; }
        public int TotalReviews { get; set; }
        public int AvailableBooks { get; set; }
        public decimal TotalFinesCollected { get; set; }

        public IEnumerable<BookViewModel> RecentBooks { get; set; } = new List<BookViewModel>();
        public IEnumerable<BookLoanViewModel> RecentLoans { get; set; } = new List<BookLoanViewModel>();
        public IEnumerable<CustomerViewModel> NewCustomers { get; set; } = new List<CustomerViewModel>();
        public IEnumerable<BookViewModel> PopularBooks { get; set; } = new List<BookViewModel>();

        public Dictionary<string, int> BooksByCategory { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> LoansByMonth { get; set; } = new Dictionary<string, int>();
    }
}