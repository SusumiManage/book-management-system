using BookManagement.Api.Data;
using BookManagement.Api.Dtos;
using BookManagement.Api.Helpers;
using BookManagement.Api.Models;
using BookManagement.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Api.Services
{
    /// <summary>
    /// Service responsible for business logic related to book borrowing.
    /// Handles borrow, return, overdue checks, and active borrow retrieval.
    /// </summary>
    public class BookBorrowedDetailService : IBookBorrowedDetailService
    {
        private readonly AppDbContext _db; 
        private readonly IBookBorrowedDetailRepository _repo;

        public BookBorrowedDetailService(AppDbContext db, IBookBorrowedDetailRepository repo)
        {
            _db = db;
            _repo = repo;
        }

        // Borrow a book (Applies business validations before creating the borrow record)
        public async Task<(bool ok, string? error, BookBorrowedDetailResponseDto? data)> BorrowAsync(
            BorrowBookRequestDto dto, int issuedByUserId)
        {
            // validate book exists
            var bookExists = await _db.Books.AnyAsync(b => b.Id == dto.BookId);
            if (!bookExists) return (false, "Book not found.", null);

            // validate borrower exists
            var borrowerExists = await _db.Users.AnyAsync(u => u.Id == dto.BorrowedByUserId);
            if (!borrowerExists) return (false, "Borrower user not found.", null);

            // validate issuer exists
            var issuerExists = await _db.Users.AnyAsync(u => u.Id == issuedByUserId);
            if (!issuerExists) return (false, "Issuer user not found.", null);

            // prevent double-borrow
            var active = await _repo.GetActiveByBookIdAsync(dto.BookId);
            if (active != null) return (false, "This book is already borrowed.", null);

            // Check User has overdue books
            var now = DateTime.UtcNow;

            var hasOverdue = await _repo.Query().AnyAsync(x =>
                x.BorrowedByUserId == dto.BorrowedByUserId &&
                x.ReturnedAt == null &&
                x.DueAt != null &&
                x.DueAt < now);

            if (hasOverdue)
                return (false,
                    "User has overdue books. Please return overdue books before borrowing another.",
                    null);

            // Create borrow record
            var entity = new BookBorrowedDetail
            {
                BookId = dto.BookId,
                BorrowedByUserId = dto.BorrowedByUserId,
                IssuedByUserId = issuedByUserId,
                BorrowedAt = now,
                IssuedAt = now,
                DueAt = dto.DueAt
            };

            // Save to database
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            // Retrieve saved active record
            var saved = await _repo.GetActiveByBookIdAsync(dto.BookId);
            if (saved == null) return (true, null, null);

            return (true, null, Map(saved));
        }

        // Return a borrowed book
        public async Task<(bool ok, string? error)> ReturnAsync(int bookId, int issuedByUserId)
        {
            // Validate issuer exists
            var issuerExists = await _db.Users.AnyAsync(u => u.Id == issuedByUserId);
            if (!issuerExists) return (false, "Issuer user not found.");

            // Get active borrow record
            var active = await _repo.GetActiveByBookIdAsync(bookId);
            if (active == null) return (false, "Active borrow record not found for this book.");

            // MUpdate as returned
            active.ReturnedAt = DateTime.UtcNow;

            await _repo.SaveChangesAsync();
            return (true, null);
        }

        // Get all active borrow records (books not yet returned)
        public async Task<List<BookBorrowedDetailResponseDto>> GetAllActiveAsync()
        {
            var active = await _repo.GetAllActiveAsync();
            return active.Select(Map).ToList();
        }

        // Get active borrow record by bookId
        public async Task<BookBorrowedDetailResponseDto?> GetActiveAsync(int bookId)
        {
            var active = await _repo.GetActiveByBookIdAsync(bookId);
            return active == null ? null : Map(active);
        }

        // Maps entity to response DTO.
        private static BookBorrowedDetailResponseDto Map(BookBorrowedDetail x)
            => new BookBorrowedDetailResponseDto
            {
                Id = x.Id,
                BookId = x.BookId ?? 0,
                BorrowedByUserId = x.BorrowedByUserId,
                BorrowedByUsername = x.BorrowedByUser?.Username,
                IssuedByUserId = x.IssuedByUserId,
                IssuedByUsername = x.IssuedByUser?.Username,
                BorrowedAt = x.BorrowedAt,
                IssuedAt = x.IssuedAt,
                DueAt = x.DueAt,
                ReturnedAt = x.ReturnedAt
            };

        // Retrieves paginated overdue borrow records.
        public async Task<PagedResult<OverdueBorrowBooksDto>> GetOverdueAsync(
                int pageNumber, int pageSize, string? search, int? userId)
        {
            var now = DateTime.UtcNow;

            var q = _repo.Query()
                .AsNoTracking()
                .Where(x => x.ReturnedAt == null && x.DueAt != null && x.DueAt < now);

            // Filter by specific user
            if (userId.HasValue)
                q = q.Where(x => x.BorrowedByUserId == userId.Value);

            // Search by title, ISBN, or username
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(x =>
                    x.Book.Title.ToLower().Contains(s) ||
                    (x.Book.ISBN != null && x.Book.ISBN.ToLower().Contains(s)) ||
                    x.BorrowedByUser.Username.ToLower().Contains(s));
            }

            var totalCount = await q.CountAsync();

            // Get paginated results
            var items = await q
                .OrderByDescending(x => x.DueAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new OverdueBorrowBooksDto
                {
                    BorrowId = x.Id,
                    BookId = x.BookId,
                    BookTitle = x.Book.Title,
                    Isbn = x.Book.ISBN,
                    BorrowedByUserId = x.BorrowedByUserId,
                    BorrowedByUsername = x.BorrowedByUser.Username,
                    BorrowedAt = x.BorrowedAt,
                    DueAt = x.DueAt,
                    DaysOverdue = 0
                })
                .ToListAsync();

            // Calculate overdue days
            foreach (var it in items)
                it.DaysOverdue = it.DueAt.HasValue ? (int)Math.Floor((now - it.DueAt.Value).TotalDays) : 0;

            return new PagedResult<OverdueBorrowBooksDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        // Returns total count of overdue borrow books
        public async Task<int> GetOverdueCountAsync(int? userId)
        {
            var now = DateTime.UtcNow;

            var q = _repo.Query()
                .AsNoTracking()
                .Where(x => x.ReturnedAt == null && x.DueAt != null && x.DueAt < now);

            if (userId.HasValue)
                q = q.Where(x => x.BorrowedByUserId == userId.Value);

            return await q.CountAsync();
        }

    }
}
