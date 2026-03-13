using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Data
{
    public class LibraryDbContext : IdentityDbContext<ApplicationUser>
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
        }

        // Project (a) Entities
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Book> Books => Set<Book>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<LibraryBranch> LibraryBranches => Set<LibraryBranch>();
        
        // Extras
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<BookLoan> BookLoans => Set<BookLoan>();
        public DbSet<Review> Reviews => Set<Review>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Author
            modelBuilder.Entity<Author>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => new { e.FirstName, e.LastName });
            });

            // Book
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(300);
                entity.Property(e => e.ISBN).IsRequired().HasMaxLength(20);
                entity.HasIndex(e => e.ISBN).IsUnique();
                entity.HasIndex(e => e.Title);

                entity.HasOne(e => e.Author)
                    .WithMany(a => a.Books)
                    .HasForeignKey(e => e.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Books)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.LibraryBranch)
                    .WithMany(l => l.Books)
                    .HasForeignKey(e => e.LibraryBranchId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Customer
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.LibraryCardNumber).IsUnique();

                entity.HasOne(e => e.PreferredBranch)
                    .WithMany(l => l.Customers)
                    .HasForeignKey(e => e.PreferredBranchId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // LibraryBranch
            modelBuilder.Entity<LibraryBranch>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BranchName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
                entity.HasIndex(e => e.BranchName);
            });

            // Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // BookLoan
            modelBuilder.Entity<BookLoan>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Book).WithMany(b => b.BookLoans).HasForeignKey(e => e.BookId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Customer).WithMany(c => c.BookLoans).HasForeignKey(e => e.CustomerId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.LibraryBranch).WithMany(l => l.BookLoans).HasForeignKey(e => e.LibraryBranchId).OnDelete(DeleteBehavior.SetNull);
            });

            // Review
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Book).WithMany(b => b.Reviews).HasForeignKey(e => e.BookId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Customer).WithMany(c => c.Reviews).HasForeignKey(e => e.CustomerId).OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.BookId, e.CustomerId }).IsUnique();
            });
        }
    }
}