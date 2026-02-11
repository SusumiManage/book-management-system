using BookManagement.Api.Dtos;
using BookManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookManagement.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _service;

        public BooksController(IBookService service)
        {
            _service = service;
        }

     
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? genre = null,
            [FromQuery] int? yearFrom = null,
            [FromQuery] int? yearTo = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] bool? isAvailable = null, 
            [FromQuery] bool? isDeleted = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var result = await _service.GetAllAsync(
                pageNumber,
                pageSize,
                search,
                genre,
                yearFrom,
                yearTo,
                minPrice,
                maxPrice,
                isAvailable,
                isDeleted
            );
            return Ok(result);
        }

        // Book by id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var book = await _service.GetByIdAsync(id);
            if (book == null) return NotFound(new { message = "Book not found." });
            return Ok(book);
        }

        // Add a book
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookCreateDto dto)
        {
            var (ok, error, book) = await _service.CreateAsync(dto);
            if (!ok) return BadRequest(new { message = error });

            return CreatedAtAction(nameof(GetById), new { id = book!.Id }, book);
        }

        // Update book
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] BookUpdateDto dto)
        {
            var (ok, error) = await _service.UpdateAsync(id, dto);
            if (!ok)
            {
                if (error == "Book not found.") return NotFound(new { message = error });
                return BadRequest(new { message = error });
            }

            return NoContent();
        }

        // Delete book
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var uid = User.FindFirst("uid")?.Value;

            if (!int.TryParse(uid, out var userId))
                return Unauthorized("User ID (uid) not found/invalid in token.");

            var (ok, error) = await _service.DeleteAsync(id, userId);
            if (!ok) return NotFound(new { message = error });

            return NoContent();
        }

        // Restore Book
        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var uid = User.FindFirst("uid")?.Value;

            if (!int.TryParse(uid, out var userId))
                return Unauthorized("User ID (uid) not found/invalid in token.");

            var (ok, error) = await _service.RestoreAsync(id, userId);
            if (!ok)
            {
                if (error == "Book not found.") return NotFound(new { message = error });
                return BadRequest(new { message = error });
            }

            return NoContent();
        }



    }
}
