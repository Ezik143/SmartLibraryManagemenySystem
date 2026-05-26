using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SmartLib.Data;
using SmartLib.Interfaces;
using SmartLib.Models.Dto;
using SmartLib.Models.Entities;
using SmartLib.Models.Settings;

namespace SmartLib.Repository
{
    // BorrowRecordRespository - Implements borrow record data access operations
    public class BorrowRecordRespository : IBorrowRecordRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly FineSettings _fineSettings;

        public BorrowRecordRespository(
            ApplicationDbContext context,
            IMapper mapper,
            IOptions<FineSettings> fineOptions)
        {
            _context = context;
            _mapper = mapper;
            _fineSettings = fineOptions.Value;
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

            var User = await _context.Users.FindAsync(request.UserId);

            if (User == null)
            {
                throw new KeyNotFoundException($"User with ID {request.UserId} not found.");
            }
            var borrowLimit = User.Role switch
            {
                UserRole.Teacher => 5,
                UserRole.Student => 3,
                _ => 3
            };

            int activeBorrows = await _context.BorrowRecords.CountAsync(br => br.UserId == request.UserId && br.Status == BorrowRecordStatus.BORROWED);

            if (activeBorrows >= borrowLimit)
            {
                throw new InvalidOperationException($"Borrowing limit reached. You can only borrow {borrowLimit} books at a time.");
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
                int days = Math.Min(overdueDays, _fineSettings.MaxDays);
                decimal amount = days * _fineSettings.RatePerDay;

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
