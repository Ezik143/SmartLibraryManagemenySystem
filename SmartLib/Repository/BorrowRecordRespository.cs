using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartLib.Data;
using SmartLib.Interfaces;
using SmartLib.Models.Dto;
using SmartLib.Models.Entities;

namespace SmartLib.Repository
{
    // BorrowRecordRespository - Implements borrow record data access operations
    public class BorrowRecordRespository : IBorrowRecordRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BorrowRecordRespository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BorrowRecordDto>> GetAllBorrowRecordAsync()
        {
            var entity = await _context.BorrowRecords.ToListAsync();
            var dto = _mapper.Map<IEnumerable<BorrowRecordDto>>(entity);
            return dto;
        }

        public async Task<BorrowRecordDto> GetBorrowRecordByIdAsync(int id)
        {
            // Find borrow record by ID
            var entity = await _context.BorrowRecords.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"BorrowRecord with ID {id} not found.");
            }
            var dto = _mapper.Map<BorrowRecordDto>(entity);
            return dto;
        }

        public async Task<BorrowRecordDto> GetBorrowRecordWithFineAsync(int id)
        {
            // Include related fines when fetching borrow record
            var borrowRecord = await _context.BorrowRecords
                .Include(br => br.Fines)
                .FirstOrDefaultAsync(br => br.BorrowRecordId == id);

            if (borrowRecord == null)
            {
                throw new KeyNotFoundException($"BorrowRecord with ID {id} not found.");
            }

            var response = _mapper.Map<BorrowRecordDto>(borrowRecord);
            return response;
        }

        public async Task<BorrowRecordDto> CreateBorrowRecordAsync(BorrowRecordDto request)
        {
            // Validate incoming request
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "BorrowRecord data cannot be null.");
            }

            // Verify book exists
            var book = await _context.Books.FindAsync(request.BookId);
            if (book == null)
            {
                throw new KeyNotFoundException($"Book with ID {request.BookId} not found.");
            }

            // Check book availability
            if (book.AvailableCopies <= 0)
            {
                throw new InvalidOperationException("No more copies available.");
            }

            // Decrement available copies for the book
            book.AvailableCopies -= 1;

            var entity = _mapper.Map<BorrowRecord>(request);
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<BorrowRecordDto>(entity);
            return dto;
        }

        public async Task<BorrowRecordDto> UpdateBorrowRecordAsync(int id, BorrowRecordDto request)
        {
            // Validate incoming request
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "BorrowRecord data cannot be null.");
            }

            // Find existing borrow record
            var entity = await _context.BorrowRecords.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"BorrowRecord with ID {id} not found.");
            }

            _mapper.Map(request, entity);
            await _context.SaveChangesAsync();
            var dto = _mapper.Map<BorrowRecordDto>(entity);
            return dto;
        }

        public async Task DeleteBorrowRecordAsync(int id)
        {
            // Find borrow record by ID
            var entity = await _context.BorrowRecords.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"BorrowRecord with ID {id} not found.");
            }

            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<BorrowRecordDto> ReturnBookAsync(int id)
        {
            // Load borrow record with related book data
            var entity = await _context.BorrowRecords.Include(b => b.Book)
                                                     .FirstOrDefaultAsync(br => br.BorrowRecordId == id);

            if (entity == null)
            {
                throw new KeyNotFoundException($"BorrowRecord with ID {id} not found.");
            }

            // Prevent double return
            if (entity.ReturnDate != null)
            {
                throw new InvalidOperationException("Book is already returned.");
            }

            if (entity.Book == null)
            {
                throw new KeyNotFoundException($"Book not found for BorrowRecord with ID {id}.");
            }

            // Set return date and update status
            entity.ReturnDate = DateTime.Now;
            entity.Status = BorrowRecordStatus.RETURNED;
            entity.Book.AvailableCopies += 1;

            // Calculate fine if overdue (due date passed)
            if (entity.DueDate < entity.ReturnDate.Value)
            {
                // Calculate overdue days
                int overdueDays = (entity.ReturnDate.Value - entity.DueDate).Days;
                int finerate = 3; // Daily fine rate
                int max = 30;    // Maximum days for fine calculation
                int days = Math.Min(overdueDays, max);
                decimal amount = finerate * days;

                // Create new fine record
                var fine = new Fine
                {
                    UserId = entity.UserId,
                    RecordId = entity.BorrowRecordId,
                    Amount = amount,
                    Status = FineStatus.PENDING,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Fines.AddAsync(fine);
            }

            await _context.SaveChangesAsync();

            var dto = _mapper.Map<BorrowRecordDto>(entity);
            return dto;
        }

        public async Task<IEnumerable<BorrowRecordDto>> GetAllOverdueRecordAsync()
        {
            // Filter records by OVERDUE status
            var entities = await _context.BorrowRecords.Where(s => s.Status == BorrowRecordStatus.OVERDUE)
                                                .ToListAsync();
            var dto = _mapper.Map<IEnumerable<BorrowRecordDto>>(entities);
            return dto;
        }

        public async Task<IEnumerable<BorrowRecordDto>> GetAllPaidRecordAsync()
        {
            // Filter records by RETURNED status (books returned with any fines paid)
            var entities = await _context.BorrowRecords.Where(s => s.Status == BorrowRecordStatus.RETURNED)
                                                .ToListAsync();
            var dto = _mapper.Map<IEnumerable<BorrowRecordDto>>(entities);
            return dto;
        }
    }
}
