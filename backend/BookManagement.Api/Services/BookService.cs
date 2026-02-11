using BookManagement.Api.Dtos;
using BookManagement.Api.Helpers;
using BookManagement.Api.Models;
using BookManagement.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Api.Services
{
    /// <summary>
    /// Service responsible for business logic related to books.
    /// Handles CRUD operations, filtering, availability checks,
    /// and soft delete/restore functionality.
    /// </summary>
    public class BookService : IBookService
    {
        private readonly IBookRepository _repo;
        private readonly IBookBorrowedDetailRepository _borrowRepo;

        public BookService(IBookRepository repo, IBookBorrowedDetailRepository borrowRepo)
        {
            _repo = repo;
            _borrowRepo = borrowRepo;
        }

        // Retrieves a single book by Id. Also determines whether the book is currently borrowed.
        public async Task<BookReadDto?> GetByIdAsync(int id)
        {
            var book = await _repo.GetByIdAsync(id);

            if (book == null) return null;

            // Check if there is an active borrow record
            var hasActiveBorrow = await _borrowRepo.Query()
                .AnyAsync(x => x.BookId == id && x.ReturnedAt == null);

            return MapToReadDto(book, hasActiveBorrow);
        }

        // Retrieves paginated list of books with filtering and search options
        public async Task<PagedResult<BookReadDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            string? search,
            string? genre,
            int? yearFrom,
            int? yearTo,
            decimal? minPrice,
            decimal? maxPrice,
            bool? isAvailable = null,
            bool? isDeleted = null)
        {
            
            var query = _repo.Query().IgnoreQueryFilters();

            // Search (Title/Author/Genre)
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(b =>
                    b.Title.ToLower().Contains(s) ||
                    b.Author.ToLower().Contains(s) ||
                    b.Genre.ToLower().Contains(s));
            }

            // Filter: Genre
            if (!string.IsNullOrWhiteSpace(genre))
            {
                var g = genre.Trim().ToLower();
                query = query.Where(b => b.Genre.ToLower() == g);
            }

            // Filter: Publication year range
            if (yearFrom.HasValue) query = query.Where(b => b.PublicationYear >= yearFrom.Value);
            if (yearTo.HasValue) query = query.Where(b => b.PublicationYear <= yearTo.Value);

            // Filter: Price range
            if (minPrice.HasValue) query = query.Where(b => b.Price >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(b => b.Price <= maxPrice.Value);

            // Filter: Deleted (true = only deleted, false = only active)
            if (isDeleted.HasValue)
                query = query.Where(b => b.IsDeleted == isDeleted.Value);

            // Query active borrow records
            var activeBorrowBookIds = _borrowRepo.Query()
                .Where(x => x.ReturnedAt == null)
                .Select(x => x.BookId);

            // Filter by availability
            if (isAvailable.HasValue)
            {
                query = isAvailable.Value
                    // Available = NOT deleted AND NOT borrowed
                    ? query.Where(b => !b.IsDeleted && !activeBorrowBookIds.Contains(b.Id))
                    // Unavailable = deleted OR borrowed
                    : query.Where(b => b.IsDeleted || activeBorrowBookIds.Contains(b.Id));
            }

            // Sorting by title
            query = query.OrderBy(b => b.Title);

            // Pagination
            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookReadDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Genre = b.Genre,
                    PublicationYear = b.PublicationYear,
                    ISBN = b.ISBN,
                    Price = b.Price,
                    IsDeleted = b.IsDeleted,

                    IsAvailable = b.IsDeleted
                        ? false
                        : !activeBorrowBookIds.Contains(b.Id),
                })
                .ToListAsync();

            return new PagedResult<BookReadDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        // Creates a new book.Ensures ISBN uniqueness and prevents duplicates
        public async Task<(bool ok, string? error, BookReadDto? book)> CreateAsync(BookCreateDto dto)
        {
            // Check if ISBN already exists
            var existing = await _repo.GetByIsbnAsync(dto.ISBN.Trim());
            if (existing != null)
            {
                // If the existing book is soft-deleted
                if (existing.IsDeleted)
                    return (false, "This ISBN exists but the book is deleted. Please restore it instead.", null);

                return (false, "ISBN already exists.", null);
            }

            // Create new book entity
            var book = new Book
            {
                Title = dto.Title.Trim(),
                Author = dto.Author.Trim(),
                Genre = dto.Genre.Trim(),
                PublicationYear = dto.PublicationYear,
                ISBN = dto.ISBN.Trim(),
                Price = dto.Price
            };

            await _repo.AddAsync(book);
            await _repo.SaveChangesAsync();

            return (true, null, MapToReadDto(book, hasActiveBorrow: false));
        }

        // Updates an existing book. Ensures ISBN uniqueness if changed
        public async Task<(bool ok, string? error)> UpdateAsync(int id, BookUpdateDto dto)
        {
            var book = await _repo.GetByIdAsync(id);
            if (book == null) return (false, "Book not found.");

            var isbn = dto.ISBN.Trim();
            // Check ISBN conflict only if ISBN changed
            if (!string.Equals(book.ISBN, isbn, StringComparison.OrdinalIgnoreCase))
            {
                var existing = await _repo.GetByIsbnAsync(isbn);
                if (existing != null) return (false, "ISBN already exists.");
            }

            // Update fields
            book.Title = dto.Title.Trim();
            book.Author = dto.Author.Trim();
            book.Genre = dto.Genre.Trim();
            book.PublicationYear = dto.PublicationYear;
            book.ISBN = isbn;
            book.Price = dto.Price;

            await _repo.UpdateAsync(book);
            await _repo.SaveChangesAsync();

            return (true, null);
        }

        // Soft deletes a book. The book is marked as deleted instead of being removed.
        public async Task<(bool ok, string? error)> DeleteAsync(int id, int deletedByUserId)
        {
            var book = await _repo.GetByIdAsync(id);
            if (book == null) return (false, "Book not found.");

            book.IsDeleted = true;
            book.DeletedAt = DateTime.Now;
            book.DeletedByUserId = deletedByUserId;

            await _repo.SaveChangesAsync();

            return (true, null);
        }

        // Restores a book.
        public async Task<(bool ok, string? error)> RestoreAsync(int id, int restoredByUserId)
        {
            var book = await _repo.GetByIdAsync(id);
            if (book == null) return (false, "Book not found.");

            if (!book.IsDeleted) return (false, "Book is not deleted.");

            book.IsDeleted = false;
            book.DeletedAt = null;
            book.DeletedByUserId = null;

            await _repo.SaveChangesAsync();
            return (true, null);
        }

        // Maps Book entity to BookReadDto
        private static BookReadDto MapToReadDto(Book book, bool hasActiveBorrow) => new()
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Genre = book.Genre,
            PublicationYear = book.PublicationYear,
            ISBN = book.ISBN,
            Price = book.Price,
            IsDeleted=book.IsDeleted,
            IsAvailable = book.IsDeleted
                        ? false
                        : !hasActiveBorrow,
        };
    }
}
