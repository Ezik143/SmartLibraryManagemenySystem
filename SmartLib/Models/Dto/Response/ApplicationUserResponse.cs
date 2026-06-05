using SmartLib.Models.Entities;

namespace SmartLib.Models.Dto.Response
{
    public class ApplicationUserResponse
    {
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsAdmin { get; set; }
        public Department Department { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime? ExpireAt { get; set; }
    }
}
