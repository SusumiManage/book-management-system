namespace BookManagement.Api.Dtos
{
    public class OverdueBorrowBooksDto
    {
        public int BorrowId { get; set; }
        public int? BookId { get; set; }
        public string BookTitle { get; set; } = "";
        public string? Isbn { get; set; }
        public int BorrowedByUserId { get; set; }
        public string BorrowedByUsername { get; set; } = "";
        public DateTime BorrowedAt { get; set; }
        public DateTime? DueAt { get; set; }
        public int DaysOverdue { get; set; }
    }
}
