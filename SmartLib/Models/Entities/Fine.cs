namespace SmartLib.Models.Entities;

public class Fine
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
    public int RecordId { get; set; }
    public BorrowRecord? Record { get; set; }
    public decimal Amount { get; set; }
    public FineStatus Status { get; set; } = FineStatus.PENDING;
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }
}