using SmartLib.Models.Entities;

namespace SmartLib.Models.Dto.Response
{
    public class TeacherResponseDto
    {
        public string UserId { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
