using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(LibraryDbContext context)
        {
            if (await context.Authors.AnyAsync())
            {
                return; // Database already seeded
            }

            // Seed Library Branches first (needed for foreign keys)
            var branches = GetLibraryBranches();
            await context.LibraryBranches.AddRangeAsync(branches);
            await context.SaveChangesAsync();

            // Seed Authors
            var authors = GetAuthors();
            await context.Authors.AddRangeAsync(authors);
            await context.SaveChangesAsync();

            // Seed Books
            var books = GetBooks();
            await context.Books.AddRangeAsync(books);
            await context.SaveChangesAsync();

            // Seed Customers
            var customers = GetCustomers();
            await context.Customers.AddRangeAsync(customers);
            await context.SaveChangesAsync();
        }

        private static List<LibraryBranch> GetLibraryBranches()
        {
            return new List<LibraryBranch>
            {
                new LibraryBranch { BranchName = "Central Library", Address = "123 Main Street", City = "New York", State = "NY", PostalCode = "10001", PhoneNumber = "+1-212-555-0101", Email = "central@library.com", OpeningHours = "Mon-Sat: 9AM-9PM, Sun: 10AM-6PM" },
                new LibraryBranch { BranchName = "Downtown Branch", Address = "456 Commerce Ave", City = "New York", State = "NY", PostalCode = "10002", PhoneNumber = "+1-212-555-0102", Email = "downtown@library.com", OpeningHours = "Mon-Fri: 8AM-8PM" },
                new LibraryBranch { BranchName = "Westside Community Library", Address = "789 Oak Boulevard", City = "Los Angeles", State = "CA", PostalCode = "90001", PhoneNumber = "+1-310-555-0103", Email = "westside@library.com", OpeningHours = "Mon-Sat: 10AM-8PM" },
                new LibraryBranch { BranchName = "Eastgate Library", Address = "321 Pine Street", City = "Chicago", State = "IL", PostalCode = "60601", PhoneNumber = "+1-312-555-0104", Email = "eastgate@library.com", OpeningHours = "Mon-Fri: 9AM-7PM" },
                new LibraryBranch { BranchName = "University District Library", Address = "555 College Road", City = "Boston", State = "MA", PostalCode = "02101", PhoneNumber = "+1-617-555-0105", Email = "university@library.com", OpeningHours = "Mon-Sun: 7AM-11PM" },
                new LibraryBranch { BranchName = "Suburban Hills Branch", Address = "888 Valley Drive", City = "San Francisco", State = "CA", PostalCode = "94101", PhoneNumber = "+1-415-555-0106", Email = "suburban@library.com", OpeningHours = "Mon-Sat: 9AM-6PM" },
                new LibraryBranch { BranchName = "Riverside Reading Center", Address = "222 River Road", City = "Portland", State = "OR", PostalCode = "97201", PhoneNumber = "+1-503-555-0107", Email = "riverside@library.com", OpeningHours = "Mon-Fri: 10AM-7PM" },
                new LibraryBranch { BranchName = "Heritage Branch", Address = "444 Historic Lane", City = "Philadelphia", State = "PA", PostalCode = "19101", PhoneNumber = "+1-215-555-0108", Email = "heritage@library.com", OpeningHours = "Mon-Sat: 9AM-5PM" },
                new LibraryBranch { BranchName = "Modern Media Library", Address = "666 Digital Drive", City = "Seattle", State = "WA", PostalCode = "98101", PhoneNumber = "+1-206-555-0109", Email = "modern@library.com", OpeningHours = "Mon-Sun: 8AM-10PM" },
                new LibraryBranch { BranchName = "Children's Discovery Library", Address = "777 Rainbow Street", City = "Denver", State = "CO", PostalCode = "80201", PhoneNumber = "+1-303-555-0110", Email = "children@library.com", OpeningHours = "Mon-Sat: 9AM-6PM" },
                new LibraryBranch { BranchName = "Tech Hub Library", Address = "999 Innovation Way", City = "Austin", State = "TX", PostalCode = "73301", PhoneNumber = "+1-512-555-0111", Email = "techhub@library.com", OpeningHours = "Mon-Fri: 8AM-9PM" },
                new LibraryBranch { BranchName = "Arts & Culture Library", Address = "111 Gallery Avenue", City = "Miami", State = "FL", PostalCode = "33101", PhoneNumber = "+1-305-555-0112", Email = "arts@library.com", OpeningHours = "Tue-Sun: 10AM-8PM" },
                new LibraryBranch { BranchName = "Lakeside Branch", Address = "333 Shore Drive", City = "Minneapolis", State = "MN", PostalCode = "55401", PhoneNumber = "+1-612-555-0113", Email = "lakeside@library.com", OpeningHours = "Mon-Sat: 9AM-7PM" },
                new LibraryBranch { BranchName = "Mountain View Library", Address = "555 Peak Road", City = "Salt Lake City", State = "UT", PostalCode = "84101", PhoneNumber = "+1-801-555-0114", Email = "mountain@library.com", OpeningHours = "Mon-Fri: 9AM-6PM" },
                new LibraryBranch { BranchName = "Historic Downtown Library", Address = "777 Old Town Square", City = "San Diego", State = "CA", PostalCode = "92101", PhoneNumber = "+1-619-555-0115", Email = "historic@library.com", OpeningHours = "Mon-Sat: 10AM-6PM" },
                new LibraryBranch { BranchName = "Garden District Library", Address = "888 Bloom Street", City = "New Orleans", State = "LA", PostalCode = "70112", PhoneNumber = "+1-504-555-0116", Email = "garden@library.com", OpeningHours = "Tue-Sat: 9AM-5PM" },
                new LibraryBranch { BranchName = "Academic Resource Center", Address = "100 Scholar Lane", City = "Ann Arbor", State = "MI", PostalCode = "48104", PhoneNumber = "+1-734-555-0117", Email = "academic@library.com", OpeningHours = "Mon-Sun: 6AM-12AM" },
                new LibraryBranch { BranchName = "Community Connect Library", Address = "200 Unity Plaza", City = "Nashville", State = "TN", PostalCode = "37201", PhoneNumber = "+1-615-555-0118", Email = "connect@library.com", OpeningHours = "Mon-Sat: 8AM-8PM" },
                new LibraryBranch { BranchName = "Innovation Station Library", Address = "300 Future Boulevard", City = "Phoenix", State = "AZ", PostalCode = "85001", PhoneNumber = "+1-602-555-0119", Email = "innovation@library.com", OpeningHours = "Mon-Fri: 7AM-9PM" },
                new LibraryBranch { BranchName = "Sunset Branch", Address = "400 Evening Star Drive", City = "Las Vegas", State = "NV", PostalCode = "89101", PhoneNumber = "+1-702-555-0120", Email = "sunset@library.com", OpeningHours = "Mon-Sun: 10AM-10PM" }
            };
        }

        private static List<Author> GetAuthors()
        {
            return new List<Author>
            {
                new Author { FirstName = "Stephen", LastName = "King", DateOfBirth = new DateTime(1947, 9, 21), Nationality = "American", Biography = "Stephen King is an American author of horror, supernatural fiction, suspense, and fantasy novels." },
                new Author { FirstName = "J.K.", LastName = "Rowling", DateOfBirth = new DateTime(1965, 7, 31), Nationality = "British", Biography = "J.K. Rowling is a British author best known for the Harry Potter fantasy series." },
                new Author { FirstName = "George", LastName = "Orwell", DateOfBirth = new DateTime(1903, 6, 25), Nationality = "British", Biography = "George Orwell was an English novelist, essayist, and critic." },
                new Author { FirstName = "Jane", LastName = "Austen", DateOfBirth = new DateTime(1775, 12, 16), Nationality = "British", Biography = "Jane Austen was an English novelist known for her six major novels." },
                new Author { FirstName = "Mark", LastName = "Twain", DateOfBirth = new DateTime(1835, 11, 30), Nationality = "American", Biography = "Mark Twain was an American writer, humorist, and lecturer." },
                new Author { FirstName = "Ernest", LastName = "Hemingway", DateOfBirth = new DateTime(1899, 7, 21), Nationality = "American", Biography = "Ernest Hemingway was an American novelist and short-story writer." },
                new Author { FirstName = "Agatha", LastName = "Christie", DateOfBirth = new DateTime(1890, 9, 15), Nationality = "British", Biography = "Agatha Christie was an English writer known for her detective novels." },
                new Author { FirstName = "Charles", LastName = "Dickens", DateOfBirth = new DateTime(1812, 2, 7), Nationality = "British", Biography = "Charles Dickens was an English writer and social critic." },
                new Author { FirstName = "Leo", LastName = "Tolstoy", DateOfBirth = new DateTime(1828, 9, 9), Nationality = "Russian", Biography = "Leo Tolstoy was a Russian writer regarded as one of the greatest authors of all time." },
                new Author { FirstName = "Gabriel", LastName = "García Márquez", DateOfBirth = new DateTime(1927, 3, 6), Nationality = "Colombian", Biography = "Gabriel García Márquez was a Colombian novelist and Nobel Prize winner." },
                new Author { FirstName = "Fyodor", LastName = "Dostoevsky", DateOfBirth = new DateTime(1821, 11, 11), Nationality = "Russian", Biography = "Fyodor Dostoevsky was a Russian novelist and philosopher." },
                new Author { FirstName = "Virginia", LastName = "Woolf", DateOfBirth = new DateTime(1882, 1, 25), Nationality = "British", Biography = "Virginia Woolf was an English writer and modernist." },
                new Author { FirstName = "Franz", LastName = "Kafka", DateOfBirth = new DateTime(1883, 7, 3), Nationality = "Czech", Biography = "Franz Kafka was a German-speaking Bohemian novelist." },
                new Author { FirstName = "Oscar", LastName = "Wilde", DateOfBirth = new DateTime(1854, 10, 16), Nationality = "Irish", Biography = "Oscar Wilde was an Irish poet and playwright." },
                new Author { FirstName = "Harper", LastName = "Lee", DateOfBirth = new DateTime(1926, 4, 28), Nationality = "American", Biography = "Harper Lee was an American novelist known for To Kill a Mockingbird." },
                new Author { FirstName = "F. Scott", LastName = "Fitzgerald", DateOfBirth = new DateTime(1896, 9, 24), Nationality = "American", Biography = "F. Scott Fitzgerald was an American novelist of the Jazz Age." },
                new Author { FirstName = "George R.R.", LastName = "Martin", DateOfBirth = new DateTime(1948, 9, 20), Nationality = "American", Biography = "George R.R. Martin is an American novelist known for A Song of Ice and Fire." },
                new Author { FirstName = "J.R.R.", LastName = "Tolkien", DateOfBirth = new DateTime(1892, 1, 3), Nationality = "British", Biography = "J.R.R. Tolkien was an English writer known for The Lord of the Rings." },
                new Author { FirstName = "Dan", LastName = "Brown", DateOfBirth = new DateTime(1964, 6, 22), Nationality = "American", Biography = "Dan Brown is an American author known for thriller novels." },
                new Author { FirstName = "Paulo", LastName = "Coelho", DateOfBirth = new DateTime(1947, 8, 24), Nationality = "Brazilian", Biography = "Paulo Coelho is a Brazilian lyricist and novelist." },
                new Author { FirstName = "Margaret", LastName = "Atwood", DateOfBirth = new DateTime(1939, 11, 18), Nationality = "Canadian", Biography = "Margaret Atwood is a Canadian poet, novelist, and environmental activist." },
                new Author { FirstName = "Neil", LastName = "Gaiman", DateOfBirth = new DateTime(1960, 11, 10), Nationality = "British", Biography = "Neil Gaiman is an English author of short fiction, novels, and comics." },
                new Author { FirstName = "Khaled", LastName = "Hosseini", DateOfBirth = new DateTime(1965, 3, 4), Nationality = "Afghan-American", Biography = "Khaled Hosseini is an Afghan-American novelist." },
                new Author { FirstName = "Toni", LastName = "Morrison", DateOfBirth = new DateTime(1931, 2, 18), Nationality = "American", Biography = "Toni Morrison was an American novelist and Nobel Prize winner." },
                new Author { FirstName = "Haruki", LastName = "Murakami", DateOfBirth = new DateTime(1949, 1, 12), Nationality = "Japanese", Biography = "Haruki Murakami is a Japanese writer and translator." }
            };
        }

        private static List<Book> GetBooks()
        {
            return new List<Book>
            {
                new Book { Title = "The Shining", ISBN = "978-0-385-12167-5", Description = "A family heads to an isolated hotel for the winter where a sinister presence influences the father into violence.", PublicationDate = new DateTime(1977, 1, 28), Publisher = "Doubleday", PageCount = 447, Genre = "Horror", AuthorId = 1, LibraryBranchId = 1, AvailableCopies = 3, TotalCopies = 5 },
                new Book { Title = "It", ISBN = "978-1-5011-4217-4", Description = "A group of children face a shape-shifting monster that exploits their fears.", PublicationDate = new DateTime(1986, 9, 15), Publisher = "Viking", PageCount = 1138, Genre = "Horror", AuthorId = 1, LibraryBranchId = 1, AvailableCopies = 2, TotalCopies = 4 },
                new Book { Title = "Harry Potter and the Sorcerer's Stone", ISBN = "978-0-590-35340-3", Description = "A young wizard discovers his magical heritage on his eleventh birthday.", PublicationDate = new DateTime(1997, 6, 26), Publisher = "Scholastic", PageCount = 309, Genre = "Fantasy", AuthorId = 2, LibraryBranchId = 2, AvailableCopies = 5, TotalCopies = 7 },
                new Book { Title = "Harry Potter and the Chamber of Secrets", ISBN = "978-0-439-06487-3", Description = "Harry returns to Hogwarts for his second year and faces new dangers.", PublicationDate = new DateTime(1998, 7, 2), Publisher = "Scholastic", PageCount = 341, Genre = "Fantasy", AuthorId = 2, LibraryBranchId = 2, AvailableCopies = 4, TotalCopies = 6 },
                new Book { Title = "1984", ISBN = "978-0-452-28423-4", Description = "A dystopian novel set in a totalitarian society ruled by Big Brother.", PublicationDate = new DateTime(1949, 6, 8), Publisher = "Secker & Warburg", PageCount = 328, Genre = "Dystopian", AuthorId = 3, LibraryBranchId = 3, AvailableCopies = 6, TotalCopies = 8 },
                new Book { Title = "Animal Farm", ISBN = "978-0-451-52634-2", Description = "A satirical allegory of Soviet totalitarianism.", PublicationDate = new DateTime(1945, 8, 17), Publisher = "Secker & Warburg", PageCount = 112, Genre = "Satire", AuthorId = 3, LibraryBranchId = 3, AvailableCopies = 4, TotalCopies = 5 },
                new Book { Title = "Pride and Prejudice", ISBN = "978-0-14-143951-8", Description = "A romantic novel following Elizabeth Bennet as she deals with issues of manners and morality.", PublicationDate = new DateTime(1813, 1, 28), Publisher = "T. Egerton", PageCount = 432, Genre = "Romance", AuthorId = 4, LibraryBranchId = 4, AvailableCopies = 3, TotalCopies = 5 },
                new Book { Title = "The Adventures of Tom Sawyer", ISBN = "978-0-14-036673-4", Description = "A boy growing up along the Mississippi River in the mid-1800s.", PublicationDate = new DateTime(1876, 6, 1), Publisher = "American Publishing Company", PageCount = 274, Genre = "Adventure", AuthorId = 5, LibraryBranchId = 5, AvailableCopies = 2, TotalCopies = 4 },
                new Book { Title = "The Old Man and the Sea", ISBN = "978-0-684-80122-3", Description = "An aging Cuban fisherman struggles with a giant marlin.", PublicationDate = new DateTime(1952, 9, 1), Publisher = "Charles Scribner's Sons", PageCount = 127, Genre = "Literary Fiction", AuthorId = 6, LibraryBranchId = 6, AvailableCopies = 3, TotalCopies = 4 },
                new Book { Title = "Murder on the Orient Express", ISBN = "978-0-06-269366-2", Description = "Detective Hercule Poirot investigates a murder on a train.", PublicationDate = new DateTime(1934, 1, 1), Publisher = "Collins Crime Club", PageCount = 256, Genre = "Mystery", AuthorId = 7, LibraryBranchId = 7, AvailableCopies = 4, TotalCopies = 6 },
                new Book { Title = "A Tale of Two Cities", ISBN = "978-0-14-143960-0", Description = "A historical novel set during the French Revolution.", PublicationDate = new DateTime(1859, 4, 30), Publisher = "Chapman & Hall", PageCount = 489, Genre = "Historical Fiction", AuthorId = 8, LibraryBranchId = 8, AvailableCopies = 2, TotalCopies = 4 },
                new Book { Title = "War and Peace", ISBN = "978-0-14-044793-4", Description = "An epic novel of Russian society during the Napoleonic Era.", PublicationDate = new DateTime(1869, 1, 1), Publisher = "The Russian Messenger", PageCount = 1225, Genre = "Historical Fiction", AuthorId = 9, LibraryBranchId = 9, AvailableCopies = 1, TotalCopies = 3 },
                new Book { Title = "One Hundred Years of Solitude", ISBN = "978-0-06-088328-7", Description = "The multi-generational story of the Buendía family.", PublicationDate = new DateTime(1967, 5, 30), Publisher = "Harper & Row", PageCount = 417, Genre = "Magical Realism", AuthorId = 10, LibraryBranchId = 10, AvailableCopies = 3, TotalCopies = 5 },
                new Book { Title = "Crime and Punishment", ISBN = "978-0-14-044913-6", Description = "A psychological drama about a man who commits a murder.", PublicationDate = new DateTime(1866, 1, 1), Publisher = "The Russian Messenger", PageCount = 671, Genre = "Psychological Fiction", AuthorId = 11, LibraryBranchId = 11, AvailableCopies = 2, TotalCopies = 4 },
                new Book { Title = "Mrs Dalloway", ISBN = "978-0-15-662870-9", Description = "A day in the life of Clarissa Dalloway in post-World War I England.", PublicationDate = new DateTime(1925, 5, 14), Publisher = "Hogarth Press", PageCount = 194, Genre = "Modernist", AuthorId = 12, LibraryBranchId = 12, AvailableCopies = 2, TotalCopies = 3 },
                new Book { Title = "The Metamorphosis", ISBN = "978-0-393-34709-8", Description = "A man wakes up to find himself transformed into a giant insect.", PublicationDate = new DateTime(1915, 10, 1), Publisher = "Kurt Wolff Verlag", PageCount = 201, Genre = "Absurdist Fiction", AuthorId = 13, LibraryBranchId = 13, AvailableCopies = 3, TotalCopies = 4 },
                new Book { Title = "The Picture of Dorian Gray", ISBN = "978-0-14-143957-0", Description = "A man remains young while his portrait ages.", PublicationDate = new DateTime(1890, 7, 1), Publisher = "Lippincott's Monthly Magazine", PageCount = 254, Genre = "Gothic Fiction", AuthorId = 14, LibraryBranchId = 14, AvailableCopies = 4, TotalCopies = 5 },
                new Book { Title = "To Kill a Mockingbird", ISBN = "978-0-06-112008-4", Description = "A young girl in the Depression-era South witnesses her father defend a black man.", PublicationDate = new DateTime(1960, 7, 11), Publisher = "J. B. Lippincott & Co.", PageCount = 281, Genre = "Southern Gothic", AuthorId = 15, LibraryBranchId = 15, AvailableCopies = 5, TotalCopies = 7 },
                new Book { Title = "The Great Gatsby", ISBN = "978-0-7432-7356-5", Description = "A story of the mysteriously wealthy Jay Gatsby and his love for Daisy Buchanan.", PublicationDate = new DateTime(1925, 4, 10), Publisher = "Charles Scribner's Sons", PageCount = 180, Genre = "Literary Fiction", AuthorId = 16, LibraryBranchId = 16, AvailableCopies = 4, TotalCopies = 6 },
                new Book { Title = "A Game of Thrones", ISBN = "978-0-553-38168-3", Description = "Noble families vie for control of the Iron Throne.", PublicationDate = new DateTime(1996, 8, 1), Publisher = "Bantam Spectra", PageCount = 694, Genre = "Fantasy", AuthorId = 17, LibraryBranchId = 17, AvailableCopies = 3, TotalCopies = 5 },
                new Book { Title = "The Lord of the Rings: The Fellowship of the Ring", ISBN = "978-0-618-00222-8", Description = "A hobbit inherits a ring of immense power.", PublicationDate = new DateTime(1954, 7, 29), Publisher = "Allen & Unwin", PageCount = 423, Genre = "Fantasy", AuthorId = 18, LibraryBranchId = 18, AvailableCopies = 4, TotalCopies = 6 },
                new Book { Title = "The Da Vinci Code", ISBN = "978-0-385-50420-1", Description = "A symbologist uncovers a religious mystery.", PublicationDate = new DateTime(2003, 3, 18), Publisher = "Doubleday", PageCount = 454, Genre = "Thriller", AuthorId = 19, LibraryBranchId = 19, AvailableCopies = 5, TotalCopies = 7 },
                new Book { Title = "The Alchemist", ISBN = "978-0-06-112241-5", Description = "A shepherd boy travels to Egypt in search of treasure.", PublicationDate = new DateTime(1988, 1, 1), Publisher = "HarperOne", PageCount = 208, Genre = "Fantasy", AuthorId = 20, LibraryBranchId = 20, AvailableCopies = 6, TotalCopies = 8 },
                new Book { Title = "The Handmaid's Tale", ISBN = "978-0-385-49081-8", Description = "A dystopian novel set in a totalitarian theocracy.", PublicationDate = new DateTime(1985, 9, 17), Publisher = "McClelland & Stewart", PageCount = 311, Genre = "Dystopian", AuthorId = 21, LibraryBranchId = 1, AvailableCopies = 3, TotalCopies = 5 },
                new Book { Title = "American Gods", ISBN = "978-0-06-055812-9", Description = "A man becomes embroiled in a conflict between old and new gods.", PublicationDate = new DateTime(2001, 6, 19), Publisher = "William Morrow", PageCount = 541, Genre = "Fantasy", AuthorId = 22, LibraryBranchId = 2, AvailableCopies = 2, TotalCopies = 4 }
            };
        }

        private static List<Customer> GetCustomers()
        {
            return new List<Customer>
            {
                new Customer { FirstName = "Emma", LastName = "Wilson", Email = "emma.wilson@email.com", PhoneNumber = "+1-555-0101", Address = "123 Oak Street", City = "New York", LibraryCardNumber = "LIB-2024-0001", PreferredBranchId = 1 },
                new Customer { FirstName = "James", LastName = "Brown", Email = "james.brown@email.com", PhoneNumber = "+1-555-0102", Address = "456 Maple Avenue", City = "Los Angeles", LibraryCardNumber = "LIB-2024-0002", PreferredBranchId = 3 },
                new Customer { FirstName = "Olivia", LastName = "Davis", Email = "olivia.davis@email.com", PhoneNumber = "+1-555-0103", Address = "789 Pine Road", City = "Chicago", LibraryCardNumber = "LIB-2024-0003", PreferredBranchId = 4 },
                new Customer { FirstName = "William", LastName = "Johnson", Email = "william.johnson@email.com", PhoneNumber = "+1-555-0104", Address = "321 Elm Street", City = "Boston", LibraryCardNumber = "LIB-2024-0004", PreferredBranchId = 5 },
                new Customer { FirstName = "Sophia", LastName = "Miller", Email = "sophia.miller@email.com", PhoneNumber = "+1-555-0105", Address = "654 Cedar Lane", City = "San Francisco", LibraryCardNumber = "LIB-2024-0005", PreferredBranchId = 6 },
                new Customer { FirstName = "Benjamin", LastName = "Garcia", Email = "benjamin.garcia@email.com", PhoneNumber = "+1-555-0106", Address = "987 Birch Drive", City = "Portland", LibraryCardNumber = "LIB-2024-0006", PreferredBranchId = 7 },
                new Customer { FirstName = "Isabella", LastName = "Martinez", Email = "isabella.martinez@email.com", PhoneNumber = "+1-555-0107", Address = "147 Willow Way", City = "Philadelphia", LibraryCardNumber = "LIB-2024-0007", PreferredBranchId = 8 },
                new Customer { FirstName = "Lucas", LastName = "Anderson", Email = "lucas.anderson@email.com", PhoneNumber = "+1-555-0108", Address = "258 Spruce Court", City = "Seattle", LibraryCardNumber = "LIB-2024-0008", PreferredBranchId = 9 },
                new Customer { FirstName = "Mia", LastName = "Thomas", Email = "mia.thomas@email.com", PhoneNumber = "+1-555-0109", Address = "369 Hickory Lane", City = "Denver", LibraryCardNumber = "LIB-2024-0009", PreferredBranchId = 10 },
                new Customer { FirstName = "Henry", LastName = "Jackson", Email = "henry.jackson@email.com", PhoneNumber = "+1-555-0110", Address = "741 Ash Boulevard", City = "Austin", LibraryCardNumber = "LIB-2024-0010", PreferredBranchId = 11 },
                new Customer { FirstName = "Charlotte", LastName = "White", Email = "charlotte.white@email.com", PhoneNumber = "+1-555-0111", Address = "852 Sycamore Street", City = "Miami", LibraryCardNumber = "LIB-2024-0011", PreferredBranchId = 12 },
                new Customer { FirstName = "Alexander", LastName = "Harris", Email = "alexander.harris@email.com", PhoneNumber = "+1-555-0112", Address = "963 Poplar Avenue", City = "Minneapolis", LibraryCardNumber = "LIB-2024-0012", PreferredBranchId = 13 },
                new Customer { FirstName = "Amelia", LastName = "Clark", Email = "amelia.clark@email.com", PhoneNumber = "+1-555-0113", Address = "174 Chestnut Road", City = "Salt Lake City", LibraryCardNumber = "LIB-2024-0013", PreferredBranchId = 14 },
                new Customer { FirstName = "Daniel", LastName = "Lewis", Email = "daniel.lewis@email.com", PhoneNumber = "+1-555-0114", Address = "285 Magnolia Drive", City = "San Diego", LibraryCardNumber = "LIB-2024-0014", PreferredBranchId = 15 },
                new Customer { FirstName = "Harper", LastName = "Walker", Email = "harper.walker@email.com", PhoneNumber = "+1-555-0115", Address = "396 Dogwood Lane", City = "New Orleans", LibraryCardNumber = "LIB-2024-0015", PreferredBranchId = 16 },
                new Customer { FirstName = "Michael", LastName = "Hall", Email = "michael.hall@email.com", PhoneNumber = "+1-555-0116", Address = "417 Redwood Court", City = "Ann Arbor", LibraryCardNumber = "LIB-2024-0016", PreferredBranchId = 17 },
                new Customer { FirstName = "Evelyn", LastName = "Young", Email = "evelyn.young@email.com", PhoneNumber = "+1-555-0117", Address = "528 Sequoia Way", City = "Nashville", LibraryCardNumber = "LIB-2024-0017", PreferredBranchId = 18 },
                new Customer { FirstName = "Ethan", LastName = "King", Email = "ethan.king@email.com", PhoneNumber = "+1-555-0118", Address = "639 Juniper Street", City = "Phoenix", LibraryCardNumber = "LIB-2024-0018", PreferredBranchId = 19 },
                new Customer { FirstName = "Abigail", LastName = "Wright", Email = "abigail.wright@email.com", PhoneNumber = "+1-555-0119", Address = "740 Cypress Avenue", City = "Las Vegas", LibraryCardNumber = "LIB-2024-0019", PreferredBranchId = 20 },
                new Customer { FirstName = "Sebastian", LastName = "Lopez", Email = "sebastian.lopez@email.com", PhoneNumber = "+1-555-0120", Address = "851 Palm Boulevard", City = "New York", LibraryCardNumber = "LIB-2024-0020", PreferredBranchId = 2 },
                new Customer { FirstName = "Emily", LastName = "Hill", Email = "emily.hill@email.com", PhoneNumber = "+1-555-0121", Address = "962 Olive Street", City = "Los Angeles", LibraryCardNumber = "LIB-2024-0021", PreferredBranchId = 3 },
                new Customer { FirstName = "Matthew", LastName = "Scott", Email = "matthew.scott@email.com", PhoneNumber = "+1-555-0122", Address = "173 Laurel Lane", City = "Chicago", LibraryCardNumber = "LIB-2024-0022", PreferredBranchId = 4 },
                new Customer { FirstName = "Elizabeth", LastName = "Adams", Email = "elizabeth.adams@email.com", PhoneNumber = "+1-555-0123", Address = "284 Ivy Road", City = "Boston", LibraryCardNumber = "LIB-2024-0023", PreferredBranchId = 5 },
                new Customer { FirstName = "David", LastName = "Baker", Email = "david.baker@email.com", PhoneNumber = "+1-555-0124", Address = "395 Fern Drive", City = "San Francisco", LibraryCardNumber = "LIB-2024-0024", PreferredBranchId = 6 },
                new Customer { FirstName = "Aria", LastName = "Nelson", Email = "aria.nelson@email.com", PhoneNumber = "+1-555-0125", Address = "416 Sage Court", City = "Portland", LibraryCardNumber = "LIB-2024-0025", PreferredBranchId = 7 }
            };
        }
    }
}