using System.ComponentModel.DataAnnotations;

namespace BookManagement.Api.Dtos
{
    public class BorrowBookRequestDto
    {
        [Required]
        public int BookId { get; set; }

        [Required]
        public int BorrowedByUserId { get; set; }

        public DateTime? DueAt { get; set; }
    }
}
