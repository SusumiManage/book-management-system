using BookManagement.Api.Dtos;
using BookManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookManagement.Api.Controllers
{
    /// <summary>
    /// Controller responsible for book borrowing operations.
    /// Only accessible by Admin users.
    /// Handles borrow, return, active list, and overdue reporting.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class BookBorrowedDetailsController : ControllerBase
    {
        private readonly IBookBorrowedDetailService _service;

        public BookBorrowedDetailsController(IBookBorrowedDetailService service)
        {
            _service = service;
        }

        // Borrow a book
        [HttpPost("borrow")]
        public async Task<IActionResult> Borrow([FromBody] BorrowBookRequestDto dto)
        {
            var issuedByUserId = GetUserIdFromToken();

            var (ok, error, data) =
                await _service.BorrowAsync(dto, issuedByUserId);

            if (!ok)
                return BadRequest(new { message = error });

            return Ok(data);
        }

        // Return a borrowed book
        [HttpPost("return")]
        public async Task<IActionResult> Return([FromBody] ReturnBookRequestDto dto)
        {
            var issuedByUserId = GetUserIdFromToken();

            var (ok, error) =
                await _service.ReturnAsync(dto.BookId, issuedByUserId);

            if (!ok)
                return BadRequest(new { message = error });

            return NoContent();
        }

        // Retrieves all active borrow records (books not yet returned)
        [HttpGet("active")]
        public async Task<ActionResult<List<BookBorrowedDetailResponseDto>>> GetAllActive()
        {
            var list = await _service.GetAllActiveAsync();
            return Ok(list);
        }

        // Retrieves paginated list of overdue books.
        [HttpGet("overdue")]
        public async Task<IActionResult> GetOverdue(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] int? userId = null)
        {
            var result = await _service.GetOverdueAsync(pageNumber, pageSize, search, userId);
            return Ok(result);
        }

        // Retrieves total count of overdue books.
        [HttpGet("overdue/count")]
        public async Task<IActionResult> GetOverdueCount([FromQuery] int? userId = null)
        {
            var count = await _service.GetOverdueCountAsync(userId);
            return Ok(new { totalOverdue = count });
        }

        // Extracts the user id from the JWT token claims.
        private int GetUserIdFromToken()
        {
            var uid = User.FindFirst("uid")?.Value;

            if (string.IsNullOrWhiteSpace(uid))
                throw new UnauthorizedAccessException("User id (uid) claim missing.");

            if (!int.TryParse(uid, out var userId))
                throw new UnauthorizedAccessException("Invalid user id in token.");

            return userId;
        }
    }
}
