using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SmartLib.Data;
using SmartLib.Interfaces;
using SmartLib.Models.Dto;
using SmartLib.Models.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartLib.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowRecordController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IBorrowRecordRepository _borrowRecord;
        public BorrowRecordController(ApplicationDbContext context, IMapper mapper, IBorrowRecordRepository borrowRecord)
        {
            _context = context;
            _mapper = mapper;
            _borrowRecord = borrowRecord;
        }


        // GET: api/<BorrowRecordDto>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BorrowRecordDto>>> GetAllBorrowRecordController()
        {
            var borrowRecord = await _context.BorrowRecords.ToListAsync();

            int finerate = 3;
            int maxDays = 30;
            var currentDate = DateTime.Now;

            foreach (var record in borrowRecord)
            {
                if (currentDate > record.DueDate)
                {
                    int overdueDays = (currentDate - record.DueDate).Days;
                    int days = Math.Min(overdueDays, maxDays);
                    record.FineAmount = finerate * days;
                }
                else
                {
                    record.FineAmount = 0;
                }
            }

            // Save the updated fine amounts
            await _context.SaveChangesAsync();

            var response = _mapper.Map<IEnumerable<BorrowRecordDto>>(borrowRecord);
            return Ok(response);
        }

        // GET api/<BorrowRecordDto>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BorrowRecordDto>> GetBorrowRecordById(int id)
        {
            var borrowRecord = await _context.BorrowRecords.FindAsync(id);
            if (borrowRecord == null)
            {
                return NotFound();
            }
            var currentDate = DateTime.Now;

            if (borrowRecord.DueDate < currentDate)
            {

                int num = (currentDate - borrowRecord.DueDate).Days;
                int finerate = 3;
                int max = 30;
                int days = Math.Min(num, max);
                decimal fineAmount = finerate * days;
                borrowRecord.FineAmount = fineAmount;

            }

            var response = _mapper.Map<BorrowRecordDto>(borrowRecord);
            return Ok(response);
        }

        // POST api/<BorrowRecordDto>
        [HttpPost]
        public async Task<ActionResult<BorrowRecordDto>> CreateBorrowRecord(BorrowRecordDto request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            var book = await _context.Books.FindAsync(request.BookId);


            if (book == null)
            {
                return NotFound($"Book:{request.BookId} do not exist");
            }

            if (book.AvailableCopies <= 0)
            {
                return BadRequest($"no more copies.");
            }

            book.AvailableCopies -= 1;


            var Entity = _mapper.Map<BorrowRecord>(request);
            await _context.AddAsync(Entity);
            await _context.SaveChangesAsync();

            var Dto = _mapper.Map<BorrowRecordDto>(Entity);
            return Ok(Dto);
        }

        // PUT api/<BorrowRecordDto>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<BorrowRecordDto>> UpdateBorrowRecord(int id, BorrowRecordDto request)
        {
            if (request == null)
            {
                return BadRequest();
            }


            var entity = await _context.BorrowRecords.FindAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            _mapper.Map(request, entity);
            await _context.SaveChangesAsync();
            var dto = _mapper.Map<BorrowRecordDto>(entity);
            return Ok(dto);
        }

        // DELETE api/<BorrowRecordDto>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBorrowRecord(int id)
        {
            var entity = await _context.BorrowRecords.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
