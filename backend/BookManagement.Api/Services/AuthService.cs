using BookManagement.Api.Data;
using BookManagement.Api.Dtos;
using BookManagement.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookManagement.Api.Services
{
    /// <summary>
    /// Service responsible for authentication and user management.
    /// Handles registration, login, JWT token generation,
    /// and retrieval of registered users.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        // Password hasher used to securely store and verify passwords
        private readonly PasswordHasher<AppUser> _hasher = new();

        public AuthService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        // Registers a new user (Admin or User)
        public async Task<(bool ok, string? error)> RegisterAsync(RegisterDto dto)
        {
            var username = dto.Username.Trim();

            // Check if username already exists
            var exists = await _db.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower());
            if (exists) return (false, "Username already exists.");

            // Validate role
            var role = dto.Role.Trim();
            if (role != "Admin" && role != "User")
                return (false, "Role must be Admin or User.");

            // Create user entity
            var user = new AppUser
            {
                Username = username,
                Role = role
            };

            // Hash password securely
            user.PasswordHash = _hasher.HashPassword(user, dto.Password);

            // Save user
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return (true, null);
        }

        // Authenticates a user and returns a JWT token if successful
        public async Task<(bool ok, string? error, string? token)> LoginAsync(LoginDto dto)
        {
            var username = dto.Username.Trim();

            // Find user by username
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
            if (user == null) return (false, "Invalid username or password.", null);

            // Verify password against stored hash
            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
                return (false, "Invalid username or password.", null);

            // Generate JWT token
            var token = GenerateJwt(user);
            return (true, null, token);
        }

        // Generates a JWT token for an authenticated user. Includes username, role, and user id as claims.
        private string GenerateJwt(AppUser user)
        {
            var jwt = _config.GetSection("Jwt");
            var key = jwt["Key"]!;
            var issuer = jwt["Issuer"]!;
            var audience = jwt["Audience"]!;
            var expiresMinutes = int.Parse(jwt["ExpiresMinutes"]!);

            // Create claims for the token
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("uid", user.Id.ToString())
            };

            // Create signing key
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            // Create token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: creds
            );

            // Serialize token to string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Retrieves a list of all registered users
        public async Task<List<UserListDto>> GetRegisteredUsersAsync()
        {
            return await _db.Users
                .AsNoTracking()
                .OrderBy(u => u.Username)
                .Select(u => new UserListDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Role = u.Role
                })
                .ToListAsync();
        }

    }
}
