using Microsoft.AspNetCore.Mvc;
using SmartLib.Interfaces;
using SmartLib.Models.Dto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartLib.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _student;
        public StudentController(IStudentRepository student)
        {
            _student = student;
        }
        // GET: api/<StudentController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetAllStudent()
        {
            var entityDto = await _student.GetAllStudentsAsync();
            return Ok(entityDto);
        }

        // GET api/<StudentController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> GetStudentById(int id)
        {
            var entityDto = await _student.GetStudentByIdAsync(id);
            return Ok(entityDto);
        }

        // POST api/<StudentController>
        [HttpPost]
        public async Task<ActionResult<StudentDto>> CreateStudent(StudentDto request)
        {
            var entityDto = await _student.CreateStudentAsync(request);
            return NoContent();
        }

        // PUT api/<StudentController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<StudentDto>> UpdateStudent(int id, StudentDto request)
        {
            var entityDto = await _student.UpdateStudentAsync(id, request);
            return NoContent();
        }

        // DELETE api/<StudentController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStudent(int id)
        {
            await _student.DeleteStudentAsync(id);
            return NoContent();
        }
    }
}
