namespace SmartLib.Models.Dto.Create
{
    public class CreateStudentDto
    {
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string YearLevel { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
