namespace SmartLib.Models.Entities;

public class Fine
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int RecordId { get; set; }
    public BorrowRecord? Record { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = "PENDING";
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
