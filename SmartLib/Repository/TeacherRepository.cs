using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartLib.Data;
using SmartLib.Interfaces;
using SmartLib.Models.Dto.Create;
using SmartLib.Models.Dto.Response;
using SmartLib.Models.Entities;

namespace SmartLib.Repository
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public TeacherRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TeacherResponseDto>> GetAllTeachersAsync()
        {
            var entity = await _context.Teachers.ToListAsync();
            var dto = _mapper.Map<IEnumerable<TeacherResponseDto>>(entity);
            return dto;
        }

        public async Task<TeacherResponseDto> GetTeacherByIdAsync(int id)
        {
            var entity = await _context.Teachers.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Teacher with ID {id} not found.");
            }

            var dto = _mapper.Map<TeacherResponseDto>(entity);
            return dto;
        }

        public async Task<TeacherResponseDto> UpdateTeacherAsync(int id, CreateTeacherDto request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Teacher data cannot be null.");
            }

            var entity = await _context.Teachers.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Teacher with ID {id} not found.");
            }

            _mapper.Map(request, entity);
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<TeacherResponseDto>(entity);
            return dto;
        }

        public async Task DeleteTeacherAsync(int id)
        {
            var entity = await _context.Teachers.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Teacher with ID {id} not found.");
            }

            _context.Teachers.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}