using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartLib.Data;
using SmartLib.Interfaces;
using SmartLib.Models.Dto;
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

        public async Task<FineDto> CreateFineAsync(FineDto request)
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
            var dto = _mapper.Map(entity, request);
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

        public async Task<IEnumerable<FineDto>> GetAllFinesAsync()
        {
            // Retrieve all fines from database
            var entities = await _context.Fines.ToListAsync();

            var dto = _mapper.Map<IEnumerable<FineDto>>(entities);
            return dto;
        }

        public async Task<FineDto> GetFineByIdAsync(int id)
        {
            // Find fine by ID
            var entity = await _context.Fines.FindAsync(id);

            if (entity == null)
            {
                throw new KeyNotFoundException($"Fine with ID {id} not found.");
            }

            var dto = _mapper.Map<FineDto>(entity);
            return dto;
        }

        public async Task<FineDto> UpdateFineAsync(int id, FineDto request)
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

            var dto = _mapper.Map<FineDto>(entity);
            return dto;
        }

    }
}
