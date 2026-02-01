using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<LibraryBranch> LibraryBranches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Author configuration
            modelBuilder.Entity<Author>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            });

            // Book configuration
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(300);
                entity.Property(e => e.ISBN).IsRequired().HasMaxLength(20);
                entity.HasIndex(e => e.ISBN).IsUnique();

                entity.HasOne(e => e.Author)
                    .WithMany(a => a.Books)
                    .HasForeignKey(e => e.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.LibraryBranch)
                    .WithMany(l => l.Books)
                    .HasForeignKey(e => e.LibraryBranchId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Customer configuration
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();

                entity.HasOne(e => e.PreferredBranch)
                    .WithMany(l => l.Customers)
                    .HasForeignKey(e => e.PreferredBranchId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // LibraryBranch configuration
            modelBuilder.Entity<LibraryBranch>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BranchName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(300);
            });
        }
    }
}