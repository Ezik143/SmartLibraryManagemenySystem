namespace SmartLib.Models.Entities;

public class Student
{
    public int StudentId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? user { get; set; }
    public string Section { get; set; } = string.Empty;
    public string YearLevel { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}
