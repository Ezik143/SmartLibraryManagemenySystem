using Microsoft.AspNetCore.Mvc;
using SmartLib.Models.Dto;

namespace SmartLib.Interfaces
{
    public interface IStudentRepository
    {
        Task<IEnumerable<StudentDto>> GetAllStudentsAsync();
        Task<StudentDto> GetStudentByIdAsync(int id);
        Task<StudentDto> GetStudentByNameAsync(string name);
        Task<StudentDto> CreateStudentAsync(StudentDto request);
        Task<StudentDto> UpdateStudentAsync(int id, StudentDto request);
        Task DeleteStudentAsync(int id);
    }
}
