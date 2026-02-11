namespace BookManagement.Api.Dtos
{
    public class BookBorrowedDetailResponseDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }

        public int BorrowedByUserId { get; set; }
        public string? BorrowedByUsername { get; set; }

        public int IssuedByUserId { get; set; }
        public string? IssuedByUsername { get; set; }
        public DateTime BorrowedAt { get; set; }
        public DateTime IssuedAt { get; set; }

        public DateTime? DueAt { get; set; }
        public DateTime? ReturnedAt { get; set; }
    }
}
