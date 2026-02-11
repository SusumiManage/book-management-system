using System.ComponentModel.DataAnnotations;

namespace BookManagement.Api.Dtos
{
    public class BookCreateDto
    {
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
    }
}
