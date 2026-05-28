using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Interfaces;
using SmartLib.Models.Dto.Create;
using SmartLib.Models.Dto.Response;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartLib.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherRepository _teacher;
        private readonly IAuthRepository _authRepository;

        public TeacherController(ITeacherRepository teacher, IAuthRepository authRepository)
        {
            _teacher = teacher;
            _authRepository = authRepository;
        }

        // GET: api/<TeacherController>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeacherResponseDto>>> GetAllTeachers()
        {
            var entityDto = await _teacher.GetAllTeachersAsync();
            return Ok(entityDto);
        }

        // GET api/<TeacherController>/5
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherResponseDto>> GetTeacherById(int id)
        {
            var entityDto = await _teacher.GetTeacherByIdAsync(id);
            return Ok(entityDto);
        }

        // POST api/<TeacherController>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<TeacherResponseDto>> CreateTeacher(CreateTeacherDto request)
        {
            await _authRepository.RegisterTeacherAsync(request);
            return NoContent();
        }

        // PUT api/<TeacherController>/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<TeacherResponseDto>> UpdateTeacher(int id, CreateTeacherDto request)
        {
            var entityDto = await _teacher.UpdateTeacherAsync(id, request);
            return NoContent();
        }

        // DELETE api/<TeacherController>/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTeacher(int id)
        {
            await _teacher.DeleteTeacherAsync(id);
            return NoContent();
        }
    }
}
