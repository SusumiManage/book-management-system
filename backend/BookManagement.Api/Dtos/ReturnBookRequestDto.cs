using System.ComponentModel.DataAnnotations;

namespace BookManagement.Api.Dtos
{
    public class ReturnBookRequestDto
    {
        [Required]
        public int BookId { get; set; }
    }
}
