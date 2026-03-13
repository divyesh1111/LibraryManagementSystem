namespace LibraryManagementSystem.Exceptions
{
    public class BookNotFoundException : Exception
    {
        public int BookId { get; }
        public BookNotFoundException(int bookId) : base($"Book ID {bookId} not found.") { BookId = bookId; }
    }
}