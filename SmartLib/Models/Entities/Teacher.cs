namespace SmartLib.Models.Entities;

public class Teacher
{
    public int TeacherId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
    public int DepartmentId { get; set; }
    public Department Department { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}
