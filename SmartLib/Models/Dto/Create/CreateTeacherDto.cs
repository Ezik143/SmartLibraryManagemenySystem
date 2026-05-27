using SmartLib.Models.Entities;

namespace SmartLib.Models.Dto.Create
{
    public class CreateTeacherDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Department Department { get; set; }
        public int DepartmentId { get; set; }
        public bool IsActive { get; set; }
    }
}
