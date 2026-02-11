using BookManagement.Api.Dtos;
using BookManagement.Api.Helpers;

namespace BookManagement.Api.Services
{
    public interface IBookBorrowedDetailService
    {
        Task<(bool ok, string? error, BookBorrowedDetailResponseDto? data)> BorrowAsync(
        BorrowBookRequestDto dto, int issuedByUserId);

        Task<(bool ok, string? error)> ReturnAsync(int bookId, int issuedByUserId);

        Task<BookBorrowedDetailResponseDto?> GetActiveAsync(int bookId);

        Task<List<BookBorrowedDetailResponseDto>> GetAllActiveAsync();
        Task<PagedResult<OverdueBorrowBooksDto>> GetOverdueAsync(
        int pageNumber, int pageSize, string? search, int? userId);

        Task<int> GetOverdueCountAsync(int? userId);

    }
}
