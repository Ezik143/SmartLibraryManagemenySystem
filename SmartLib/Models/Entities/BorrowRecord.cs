namespace SmartLib.Models.Entities;

public class BorrowRecord
{
    public int BorrowRecordId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
    public int BookId { get; set; }
    public Book? Book { get; set; }
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public BorrowRecordStatus Status { get; set; } = BorrowRecordStatus.BORROWED;
    public DateTime CreatedAt { get; set; }
    public ICollection<Fine> Fines { get; set; } = new List<Fine>();
}