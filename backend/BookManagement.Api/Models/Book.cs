using System.ComponentModel.DataAnnotations;

namespace BookManagement.Api.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string Author { get; set; } = string.Empty;

        [Required, MaxLength(80)]
        public string Genre { get; set; } = string.Empty;

        [Range(0, 3000)]
        public int PublicationYear { get; set; }

        [Required, MaxLength(20)]
        public string ISBN { get; set; } = string.Empty;

        [Range(0, 999999)]
        public decimal Price { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public int? DeletedByUserId { get; set; }
    }
}
