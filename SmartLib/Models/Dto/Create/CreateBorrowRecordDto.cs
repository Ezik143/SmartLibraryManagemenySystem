using SmartLib.Models.Entities;

namespace SmartLib.Models.Dto.Create
{
    public class CreateBorrowRecordDto
    {
        public string UserId { get; set; } = string.Empty;
        public int BookId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public BorrowRecordStatus Status { get; set; } = BorrowRecordStatus.BORROWED;
    }
}
