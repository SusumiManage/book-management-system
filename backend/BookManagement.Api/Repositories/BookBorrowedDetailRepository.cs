using BookManagement.Api.Data;
using BookManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Api.Repositories
{
    /// <summary>
    /// Repository responsible for handling database operations
    /// related to BookBorrowedDetail entities.
    /// This follows the Repository Pattern to isolate data access logic.
    /// </summary>
    public class BookBorrowedDetailRepository : IBookBorrowedDetailRepository
    {
        private readonly AppDbContext _db;

        public BookBorrowedDetailRepository(AppDbContext db)
        {
            _db = db;
        }

        // Returns a queryable collection of all borrow records.
        public IQueryable<BookBorrowedDetail> Query()
            => _db.BookBorrowedDetails.AsQueryable();

        // Retrieves a borrow record by its Id, including related Book and User information.
        public Task<BookBorrowedDetail?> GetByIdAsync(int id)
            => _db.BookBorrowedDetails
                .Include(x => x.Book)
                .Include(x => x.BorrowedByUser)
                .Include(x => x.IssuedByUser)
                .FirstOrDefaultAsync(x => x.Id == id);

        //Retrieves the active (not yet returned) borrow record by bookId
        public Task<BookBorrowedDetail?> GetActiveByBookIdAsync(int bookId)
            => _db.BookBorrowedDetails
                .Include(x => x.Book)
                .Include(x => x.BorrowedByUser)
                .Include(x => x.IssuedByUser)
                .FirstOrDefaultAsync(x => x.BookId == bookId && x.ReturnedAt == null);

        // Add a new borrow book data
        public async Task AddAsync(BookBorrowedDetail entity)
            => await _db.BookBorrowedDetails.AddAsync(entity);

        // save changes to the database
        public Task SaveChangesAsync()
            => _db.SaveChangesAsync();

        /// Retrieves all active borrow book records (books not yet returned)
        public async Task<List<BookBorrowedDetail>> GetAllActiveAsync()
        {
            return await _db.BookBorrowedDetails
                .Include(x => x.BorrowedByUser)
                .Include(x => x.IssuedByUser)
                .Where(x => x.ReturnedAt == null)
                .ToListAsync();
        }

    }
}
