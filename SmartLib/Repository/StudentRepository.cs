using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentRepository(ApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
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

        public async Task<StudentResponseDto> CreateStudentAsync(CreateStudentDto request)
        {

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Student data cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new InvalidOperationException("Student name is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                throw new InvalidOperationException("Password is required.");
            }

            var existingUser = await _userManager.FindByNameAsync(request.Name.Trim());
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this name already exists.");
            }

            var user = new ApplicationUser
            {
                Name = request.Name.Trim(),
                UserName = request.Name.Trim(),
                Role = UserRole.Student,
                IsAdmin = false,
                CreatedAt = DateTime.UtcNow
            };

            var createUserResult = await _userManager.CreateAsync(user, request.Password);
            if (!createUserResult.Succeeded)
            {
                var errorMessage = string.Join("; ", createUserResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create user: {errorMessage}");
            }

            var entity = _mapper.Map<Student>(request);
            entity.UserId = user.Id;
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();

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
