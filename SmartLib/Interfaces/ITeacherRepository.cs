using SmartLib.Models.Dto.Create;
using SmartLib.Models.Dto.Response;

namespace SmartLib.Interfaces
{
    public interface ITeacherRepository
    {
        Task<IEnumerable<TeacherResponseDto>> GetAllTeachersAsync();
        Task<TeacherResponseDto> GetTeacherByIdAsync(int id);
        Task<TeacherResponseDto> UpdateTeacherAsync(int id, CreateTeacherDto request);
        Task DeleteTeacherAsync(int id);
    }
}