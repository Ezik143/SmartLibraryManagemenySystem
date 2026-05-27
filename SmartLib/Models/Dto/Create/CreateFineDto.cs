using SmartLib.Models.Entities;

namespace SmartLib.Models.Dto.Create
{
    public class CreateFineDto
    {
        public string UserId { get; set; } = string.Empty;
        public int RecordId { get; set; }
        public decimal Amount { get; set; }
        public FineStatus Status { get; set; } = FineStatus.PENDING;
        public DateTime? PaidAt { get; set; }
    }
}
