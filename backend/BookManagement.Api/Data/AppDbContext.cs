using BookManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books => Set<Book>();
        public DbSet<AppUser> Users => Set<AppUser>();
        public DbSet<BookBorrowedDetail> BookBorrowedDetails => Set<BookBorrowedDetail>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>()
                .HasIndex(b => b.ISBN)
                .IsUnique();

            modelBuilder.Entity<Book>()
                .Property(b => b.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Book>()
                .HasQueryFilter(b => !b.IsDeleted);

            modelBuilder.Entity<AppUser>()
               .HasIndex(u => u.Username)
               .IsUnique();

            modelBuilder.Entity<BookBorrowedDetail>(entity =>
            {
                entity.HasKey(x => x.Id);

                // Book (FK -> Books)
                entity.HasOne(x => x.Book)
                      .WithMany()
                      .HasForeignKey(x => x.BookId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);

                // BorrowedByUser (FK -> Users)
                entity.HasOne(x => x.BorrowedByUser)
                      .WithMany()
                      .HasForeignKey(x => x.BorrowedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // IssuedByUser (FK -> Users)
                entity.HasOne(x => x.IssuedByUser)
                      .WithMany()
                      .HasForeignKey(x => x.IssuedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(x => x.IssuedAt).IsRequired();
                entity.Property(x => x.BorrowedAt).IsRequired();

                //Filtered UNIQUE index in SQL Server.
                //entity.HasIndex(x => new { x.BookId, x.ReturnedAt })
                //      .HasDatabaseName("IX_BookBorrowedDetails_ActiveBorrow_BookId")
                //      .HasFilter("[ReturnedAt] IS NULL")
                //      .IsUnique();
                var index = entity.HasIndex(x => new { x.BookId, x.ReturnedAt })
                  .HasDatabaseName("IX_BookBorrowedDetails_ActiveBorrow_BookId")
                  .IsUnique();

                // Only apply filter for SQL Server
                if (Database.ProviderName == "Microsoft.EntityFrameworkCore.SqlServer")
                {
                    index.HasFilter("[ReturnedAt] IS NULL");
                }

                //Index for history queries
                entity.HasIndex(x => new { x.BookId, x.BorrowedAt })
                      .HasDatabaseName("IX_BookBorrowedDetails_BookId_BorrowedAt");
            });
        }
    }
}
