using BookManagement.Api.Dtos;
using BookManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookManagement.Api.Controllers
{
    /// <summary>
    /// Controller responsible for authentication-related endpoints.
    /// Handles login, user registration, and retrieval of registered users.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        // Login - Public
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var (ok, error, token) = await _auth.LoginAsync(dto);
            if (!ok) return Unauthorized(new { message = error });

            return Ok(new { token });
        }

        // Register a new user - Admin-only
        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var (ok, error) = await _auth.RegisterAsync(dto);
            if (!ok) return BadRequest(new { message = error });

            return Ok(new { message = "User created successfully." });
        }

        // Retrieves all registered users - Admin-only
        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public async Task<ActionResult<List<UserListDto>>> GetRegisteredUsers()
        {
            var users = await _auth.GetRegisteredUsersAsync();
            return Ok(users);
        }
    }
}
