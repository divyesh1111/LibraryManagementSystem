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
                return; 
            }

            var authors = GetAuthors();
            await context.Authors.AddRangeAsync(authors);
            await context.SaveChangesAsync();
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
    }
}