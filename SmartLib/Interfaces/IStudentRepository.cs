using Microsoft.AspNetCore.Mvc;
using SmartLib.Models.Dto;

namespace SmartLib.Interfaces
{
    public interface IStudentRepository
    {
        Task<ActionResult<IEnumerable<StudentDto>>> GetAllStudent();
        Task<ActionResult<StudentDto>> GetStudentById(int id);
        Task<ActionResult<StudentDto>> CreateStudent(StudentDto request);
        Task<ActionResult<StudentDto>> UpdateStudent(int id, StudentDto request);
        Task<ActionResult> DeleteStudent(int id);
    }
}
