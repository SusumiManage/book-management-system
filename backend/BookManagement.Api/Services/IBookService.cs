using BookManagement.Api.Dtos;
using BookManagement.Api.Helpers;

namespace BookManagement.Api.Services
{
    public interface IBookService
    {
        Task<BookReadDto?> GetByIdAsync(int id);

        Task<PagedResult<BookReadDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            string? search,
            string? genre,
            int? yearFrom,
            int? yearTo,
            decimal? minPrice,
            decimal? maxPrice,
            bool? isAvailable,
            bool? isDeleted
        );

        Task<(bool ok, string? error, BookReadDto? book)> CreateAsync(BookCreateDto dto);
        Task<(bool ok, string? error)> UpdateAsync(int id, BookUpdateDto dto);
        Task<(bool ok, string? error)> DeleteAsync(int id,int deletedByUserId);
        Task<(bool ok, string? error)> RestoreAsync(int id, int restoredByUserId);
    }
}
