using SmartLib.Models.Dto.Create;
using SmartLib.Models.Dto.Response;

namespace SmartLib.Interfaces
{
    public interface IStudentRepository
    {
        Task<IEnumerable<StudentResponseDto>> GetAllStudentsAsync();
        Task<StudentResponseDto> GetStudentByIdAsync(int id);
        Task<IEnumerable<StudentResponseDto>> GetStudentByNameAsync(string name);
        Task<StudentResponseDto> CreateStudentAsync(CreateStudentDto request);
        Task<StudentResponseDto> UpdateStudentAsync(int id, CreateStudentDto request);
        Task DeleteStudentAsync(int id);
    }
}
