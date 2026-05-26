namespace SmartLib.Models.Dto.Response
{
    public class StudentResponseDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string YearLevel { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
