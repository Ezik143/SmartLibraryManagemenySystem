using SmartLib.Models.Entities;

namespace SmartLib.Models.Dto
{
    public class FineDto
    {
        public string UserId { get; set; } = string.Empty;
        public int RecordId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = "PENDING";
        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
