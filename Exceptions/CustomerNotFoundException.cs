namespace LibraryManagementSystem.Exceptions
{
    public class CustomerNotFoundException : Exception
    {
        public int CustomerId { get; }
        public CustomerNotFoundException(int customerId) 
            : base($"Customer with ID {customerId} was not found in the system.") 
        { 
            CustomerId = customerId; 
        }
    }
}