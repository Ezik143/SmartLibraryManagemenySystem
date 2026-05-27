using SmartLib.Models.Dto.Create;
using SmartLib.Models.Dto.Response;

namespace SmartLib.Interfaces
{
    public interface IAuthRepository
    {
        Task<ApplicationUserResponse> LoginAsync(string email, string password);
        Task<StudentResponseDto> RegisterStudentAsync(CreateStudentDto request);
        Task<TeacherResponseDto> RegisterTeacherAsync(CreateTeacherDto request);
    }
}
