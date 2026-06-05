using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartLib.Data;
using SmartLib.Interfaces;
using SmartLib.Models.Dto.Create;
using SmartLib.Models.Dto.Response;
using SmartLib.Models.Entities;

namespace SmartLib.Repository
{
    // FineRepository - Implements fine data access operations
    public class FineRepository : IFineRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public FineRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<FineResponseDto> CreateFineAsync(CreateFineDto request)
        {
            // Validate input to prevent null reference exceptions
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Fine data cannot be null.");
            }

            var entity = _mapper.Map<Fine>(request);
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();

            // Map back to DTO for response
            var dto = _mapper.Map<FineResponseDto>(entity);
            return dto;
        }

        public async Task DeleteFineAsync(int id)
        {
            // Find fine by ID
            var entity = await _context.Fines.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Fine with ID {id} not found.");
            }
            _context.Fines.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<FineResponseDto>> GetAllFinesAsync()
        {
            // Retrieve all fines from database
            var entities = await _context.Fines.ToListAsync();

            var dto = _mapper.Map<IEnumerable<FineResponseDto>>(entities);
            return dto;
        }

        public async Task<IEnumerable<FineResponseDto>> GetFinesByUserIdAsync(string userId)
        {
            var entities = await _context.Fines
                .Where(f => f.UserId == userId)
                .ToListAsync();

            var dto = _mapper.Map<IEnumerable<FineResponseDto>>(entities);
            return dto;
        }

        public async Task<FineResponseDto> GetFineByIdAsync(int id)
        {
            // Find fine by ID
            var entity = await _context.Fines.FindAsync(id);

            if (entity == null)
            {
                throw new KeyNotFoundException($"Fine with ID {id} not found.");
            }

            var dto = _mapper.Map<FineResponseDto>(entity);
            return dto;
        }

        public async Task<FineResponseDto> UpdateFineAsync(int id, CreateFineDto request)
        {
            // Find existing fine to update
            var entity = await _context.Fines.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Fine with ID {id} not found.");
            }

            // Update entity properties from request
            _mapper.Map(request, entity);
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<FineResponseDto>(entity);
            return dto;
        }

    }
}
