using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartLib.Data;
using SmartLib.Interfaces;
using SmartLib.Models.Dto;
using SmartLib.Models.Entities;

namespace SmartLib.Repository
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        public StudentRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<IEnumerable<StudentDto>> GetAllStudentsAsync()
        {
            var entity = await _context.Students.ToListAsync();
            var dto = _mapper.Map<IEnumerable<StudentDto>>(entity);

            return dto;
        }

        public async Task<StudentDto> GetStudentByIdAsync(int id)
        {
            var entity = await _context.Students.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Student with ID {id} not found.");
            }

            var dto = _mapper.Map<StudentDto>(entity);
            return dto;
        }

        public async Task<StudentDto> CreateStudentAsync(StudentDto request)
        {

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Student data cannot be null.");
            }

            var result = await _context.Students.FirstOrDefaultAsync(u => u.UserId == request.UserId);
            if (result != null)
            {
                throw new InvalidOperationException("Student with this UserId already exists.");
            }


            var entity = _mapper.Map<Student>(request);
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<StudentDto>(entity);

            return dto;
        }

        public async Task DeleteStudentAsync(int id)
        {
            var entity = await _context.Students.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Student with ID {id} not found.");
            }

            _context.Students.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<StudentDto> GetStudentByNameAsync(string name)
        {
            throw new NotImplementedException("GetStudentByNameAsync method is not implemented.");
        }

        public async Task<StudentDto> UpdateStudentAsync(int id, StudentDto request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Student data cannot be null.");
            }

            var entity = await _context.Students.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Student with ID {id} not found.");
            }

            _mapper.Map(request, entity);
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<StudentDto>(entity);
            return dto;
        }
    }
}
