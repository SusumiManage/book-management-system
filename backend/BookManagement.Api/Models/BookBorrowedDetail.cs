namespace BookManagement.Api.Models
{
    public class BookBorrowedDetail
    {
        public int Id { get; set; }

        public int? BookId { get; set; }
        public Book? Book { get; set; }

        public int BorrowedByUserId { get; set; }
        public AppUser BorrowedByUser { get; set; } = null!;

        public int IssuedByUserId { get; set; }
        public AppUser IssuedByUser { get; set; } = null!;

        public DateTime BorrowedAt { get; set; }
        public DateTime IssuedAt { get; set; }

        public DateTime? DueAt { get; set; }
        public DateTime? ReturnedAt { get; set; }
    }

}
