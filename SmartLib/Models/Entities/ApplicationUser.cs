using Microsoft.AspNetCore.Identity;

namespace SmartLib.Models.Entities;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsAdmin { get; set; }
    public Department Department { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
}
