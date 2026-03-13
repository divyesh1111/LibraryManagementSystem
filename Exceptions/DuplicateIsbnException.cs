namespace LibraryManagementSystem.Exceptions
{
    public class DuplicateIsbnException : Exception
    {
        public string ISBN { get; }
        public DuplicateIsbnException(string isbn) : base($"ISBN '{isbn}' already exists.") { ISBN = isbn; }
    }
}