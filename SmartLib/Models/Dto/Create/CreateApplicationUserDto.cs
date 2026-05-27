using SmartLib.Models.Entities;

namespace SmartLib.Models.Dto.Create
{
    public class CreateApplicationUserDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsAdmin { get; set; }
        public Department Department { get; set; }
    }
}
