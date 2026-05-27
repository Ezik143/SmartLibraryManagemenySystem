using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartLib.Data;
using SmartLib.Interfaces;
using SmartLib.Models.Dto.Create;
using SmartLib.Models.Dto.Response;
using SmartLib.Models.Entities;
using System.Linq;

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


        public async Task<IEnumerable<StudentResponseDto>> GetAllStudentsAsync()
        {
            var entity = await _context.Students.ToListAsync();
            var dto = _mapper.Map<IEnumerable<StudentResponseDto>>(entity);

            return dto;
        }

        public async Task<StudentResponseDto> GetStudentByIdAsync(int id)
        {
            var entity = await _context.Students.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Student with ID {id} not found.");
            }

            var dto = _mapper.Map<StudentResponseDto>(entity);
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

        public async Task<IEnumerable<StudentResponseDto>> GetStudentByNameAsync(string name)
        {
            if (name == null)
            {
                throw new InvalidDataException($"Student cannot be empty.");
            }

            var Userentity = _context.Students
                                                        .Include(s => s.User)
                                                        .Where(s => s.User != null && s.User.Role != UserRole.Student && s.User.Name == name)
                                                        .ToList();

            var dto = _mapper.Map<IEnumerable<StudentResponseDto>>(Userentity);
            return dto;
        }

        public async Task<StudentResponseDto> UpdateStudentAsync(int id, CreateStudentDto request)
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
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<StudentResponseDto>(entity);
            return dto;
        }
    }
}
