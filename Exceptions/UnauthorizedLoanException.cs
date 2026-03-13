namespace LibraryManagementSystem.Exceptions
{
    public class UnauthorizedLoanException : Exception
    {
        public int CustomerId { get; }
        public string Reason { get; }
        
        public UnauthorizedLoanException(int customerId, string reason) 
            : base($"Customer {customerId} is not authorized to borrow books: {reason}") 
        { 
            CustomerId = customerId;
            Reason = reason;
        }
    }
}