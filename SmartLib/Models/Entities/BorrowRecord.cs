namespace SmartLib.Models.Entities;

public class BorrowRecord
{
    public int BorrowRecordId { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int BookId { get; set; }
    public Book? Book { get; set; }
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public BorrowRecordStatus Status { get; set; } = BorrowRecordStatus.BORROWED;
    public int? IssuedById { get; set; }
    public decimal FineAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}
