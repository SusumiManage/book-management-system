using System.ComponentModel.DataAnnotations;

namespace BookManagement.Api.Models
{
    public class AppUser
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // "Admin" or "User"
        [Required, MaxLength(20)]
        public string Role { get; set; } = "User";
    }
}
