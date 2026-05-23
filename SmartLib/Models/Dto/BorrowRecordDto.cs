using SmartLib.Models.Entities;

namespace SmartLib.Models.Dto
{
    public class BorrowRecordDto
    {
        public int BorrowRecordId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int BookId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public BorrowRecordStatus Status { get; set; } = BorrowRecordStatus.BORROWED;
        public DateTime CreatedAt { get; set; }

    }
}