using BookManagement.Api.Dtos;
using BookManagement.Api.Helpers;
using BookManagement.Api.Models;

namespace BookManagement.Api.Repositories
{
    public interface IBookBorrowedDetailRepository
    {
        Task<BookBorrowedDetail?> GetActiveByBookIdAsync(int bookId);
        Task<BookBorrowedDetail?> GetByIdAsync(int id);
        Task AddAsync(BookBorrowedDetail entity);
        Task SaveChangesAsync();
        IQueryable<BookBorrowedDetail> Query();
        Task<List<BookBorrowedDetail>> GetAllActiveAsync();
        
    }
}
