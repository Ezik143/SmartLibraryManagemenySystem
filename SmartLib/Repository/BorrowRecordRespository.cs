using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartLib.Data;
using SmartLib.Interfaces;
using SmartLib.Models.Dto;
using SmartLib.Models.Entities;

namespace SmartLib.Repository
{
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
            var borrowRecord = await _context.BorrowRecords.FindAsync(id);
            if (borrowRecord == null)
            {
                throw new KeyNotFoundException($"BorrowRecord with ID {id} not found.");
            }

            var currentDate = DateTime.Now;
            if (borrowRecord.DueDate < currentDate && borrowRecord.ReturnDate == null)
            {
                borrowRecord.Status = BorrowRecordStatus.OVERDUE;
                int overdueDays = (currentDate - borrowRecord.DueDate).Days;
                int finerate = 3;
                int max = 30;
                int days = Math.Min(overdueDays, max);
                borrowRecord.FineAmount = finerate * days;
            }
            else
            {
                borrowRecord.FineAmount = 0;
            }

            var response = _mapper.Map<BorrowRecordDto>(borrowRecord);
            return response;
        }

        public async Task<BorrowRecordDto> CreateBorrowRecordAsync(BorrowRecordDto request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "BorrowRecord data cannot be null.");
            }

            var book = await _context.Books.FindAsync(request.BookId);
            if (book == null)
            {
                throw new KeyNotFoundException($"Book with ID {request.BookId} not found.");
            }

            if (book.AvailableCopies <= 0)
            {
                throw new InvalidOperationException("No more copies available.");
            }

            book.AvailableCopies -= 1;

            var entity = _mapper.Map<BorrowRecord>(request);
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<BorrowRecordDto>(entity);
            return dto;
        }

        public async Task<BorrowRecordDto> UpdateBorrowRecordAsync(int id, BorrowRecordDto request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "BorrowRecord data cannot be null.");
            }

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
            var entity = await _context.BorrowRecords.Include(b => b.Book)
                                                     .FirstOrDefaultAsync(br => br.BookId == id);

            if (entity == null)
            {
                throw new KeyNotFoundException($"BorrowRecord with ID {id} not found.");
            }

            if (entity.ReturnDate != null)
            {
                throw new InvalidOperationException("Book is already returned.");
            }

            if (entity.Book == null)
            {
                throw new KeyNotFoundException($"Book not found for BorrowRecord with ID {id}.");
            }

            entity.ReturnDate = DateTime.Now;
            entity.Status = BorrowRecordStatus.RETURNED;
            entity.Book.AvailableCopies += 1;
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<BorrowRecordDto>(entity);
            return dto;
        }

        public async Task<IEnumerable<BorrowRecordDto>> GetAllOverdueRecordAsync()
        {
            var entities = await _context.BorrowRecords.Where(s => s.Status == BorrowRecordStatus.OVERDUE)
                                                .ToListAsync();
            var dto = _mapper.Map<IEnumerable<BorrowRecordDto>>(entities);
            return dto;
        }

        public async Task<IEnumerable<BorrowRecordDto>> GetAllPaidRecordAsync()
        {
            var entities = await _context.BorrowRecords.Where(s => s.Status == BorrowRecordStatus.RETURNED)
                                                .ToListAsync();
            var dto = _mapper.Map<IEnumerable<BorrowRecordDto>>(entities);
            return dto;
        }
    }
}
