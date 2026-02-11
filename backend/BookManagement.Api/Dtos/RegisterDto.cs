using System.ComponentModel.DataAnnotations;

namespace BookManagement.Api.Dtos
{
    public class RegisterDto
    {
        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;
        // "Admin" or "User"
        [Required]
        public string Role { get; set; } = "User";
    }
}
