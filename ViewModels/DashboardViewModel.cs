namespace LibraryManagementSystem.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalBooks { get; set; }
        public int TotalAuthors { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalBranches { get; set; }
        public int AvailableBooks { get; set; }
        public int BorrowedBooks { get; set; }

        public List<BookViewModel> RecentBooks { get; set; } = new List<BookViewModel>();
        public List<AuthorViewModel> TopAuthors { get; set; } = new List<AuthorViewModel>();
        public List<CustomerViewModel> RecentCustomers { get; set; } = new List<CustomerViewModel>();
    }
}