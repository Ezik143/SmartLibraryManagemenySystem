using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLib.Data;
using SmartLib.Interfaces;
using SmartLib.Models.Dto;
using SmartLib.Models.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartLib.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStudentRepository _student;
        public StudentController(ApplicationDbContext context, IMapper mapper, IStudentRepository student)
        {
            _context = context;
            _mapper = mapper;
            _student = student;
        }
        // GET: api/<StudentController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetAllStudent()
        {
            var entity = await _context.Students.ToListAsync();
            var dto = _mapper.Map<IEnumerable<StudentDto>>(entity);

            return Ok(dto);
        }

        // GET api/<StudentController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> GetStudentById(int id)
        {
            var entity = await _context.Students.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            var dto = _mapper.Map<StudentDto>(entity);
            return Ok(dto);
        }

        // POST api/<StudentController>
        [HttpPost]
        public async Task<ActionResult<StudentDto>> CreateStudent(StudentDto request)
        {

            if (request == null)
            {
                return BadRequest();
            }

            var result = await _context.Students.FirstOrDefaultAsync(u => u.UserId == request.UserId);
            if (result != null)
            {
                return Conflict("Student with this UserId already exists.");
            }


            var entity = _mapper.Map<Student>(request);
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<StudentDto>(entity);

            return Ok(dto);
        }

        // PUT api/<StudentController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<StudentDto>> UpdateStudent(int id, StudentDto request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            var entity = await _context.Students.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            _mapper.Map(request, entity);
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<StudentDto>(entity);
            return Ok(dto);
        }

        // DELETE api/<StudentController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStudent(int id)
        {
            var entity = await _context.Students.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }
            _context.Students.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
