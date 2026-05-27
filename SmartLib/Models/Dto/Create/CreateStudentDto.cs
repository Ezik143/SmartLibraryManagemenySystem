using SmartLib.Models.Entities;

namespace SmartLib.Models.Dto.Create
{
    public class CreateStudentDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Department Department { get; set; }
        public string Section { get; set; } = string.Empty;
        public string YearLevel { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
