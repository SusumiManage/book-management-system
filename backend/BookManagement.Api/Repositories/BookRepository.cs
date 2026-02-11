using BookManagement.Api.Data;
using BookManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Api.Repositories
{
    /// <summary>
    /// Repository responsible for handling database operations
    /// related to Book entities.
    /// Implements the Repository Pattern to isolate data access logic.
    /// </summary>
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _db;

        public BookRepository(AppDbContext db)
        {
            _db = db;
        }

        // Returns a queryable collection of books
        public IQueryable<Book> Query() => _db.Books.AsNoTracking();

        // Retrieves a book by Id
        public async Task<Book?> GetByIdAsync(int id)
            => await _db.Books.IgnoreQueryFilters().FirstOrDefaultAsync(b => b.Id == id);

        // Retrieves a book by ISBN
        public async Task<Book?> GetByIsbnAsync(string isbn)
            => await _db.Books.IgnoreQueryFilters().FirstOrDefaultAsync(b => b.ISBN == isbn);

        // Add a new book to the database
        public async Task AddAsync(Book book)
            => await _db.Books.AddAsync(book);

        // Update book entity
        public Task UpdateAsync(Book book)
        {
            _db.Books.Update(book);
            return Task.CompletedTask;
        }

        // Delete a book from database 
        public Task DeleteAsync(Book book)
        {
            _db.Books.Remove(book);
            return Task.CompletedTask;
        }

        // save changes to the database
        public async Task SaveChangesAsync()
            => await _db.SaveChangesAsync();

    }
}
