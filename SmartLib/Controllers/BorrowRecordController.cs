using Microsoft.AspNetCore.Mvc;
using SmartLib.Interfaces;
using SmartLib.Models.Dto.Create;
using SmartLib.Models.Dto.Response;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartLib.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowRecordController : ControllerBase
    {
        private readonly IBorrowRecordRepository _borrowRecord;
        public BorrowRecordController(IBorrowRecordRepository borrowRecord)
        {
            _borrowRecord = borrowRecord;
        }


        // GET: api/<BorrowRecordDto>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BorrowRecordResponseDto>>> GetAllBorrowRecordController()
        {
            var borrowRecord = await _borrowRecord.GetAllBorrowRecordAsync();
            return Ok(borrowRecord);
        }

        // GET api/<BorrowRecordDto>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BorrowRecordResponseDto>> GetBorrowRecordById(int id)
        {
            var borrowRecord = await _borrowRecord.GetBorrowRecordByIdAsync(id);
            return Ok(borrowRecord);
        }

        [HttpGet("fines/{id}")]
        public async Task<ActionResult<BorrowRecordResponseDto>> GetBorrowRecordWithFine(int id)
        {
            var borrowRecord = await _borrowRecord.GetBorrowRecordWithFineAsync(id);
            return Ok(borrowRecord);
        }


        // POST api/<BorrowRecordDto>
        [HttpPost]
        public async Task<ActionResult<BorrowRecordResponseDto>> CreateBorrowRecord(CreateBorrowRecordDto request)
        {
            var borrowRecord = await _borrowRecord.CreateBorrowRecordAsync(request);
            return NoContent();
        }

        // PUT api/<BorrowRecordDto>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<BorrowRecordResponseDto>> UpdateBorrowRecord(int id, CreateBorrowRecordDto request)
        {
            var borrowRecord = await _borrowRecord.UpdateBorrowRecordAsync(id, request);
            return NoContent();
        }

        // DELETE api/<BorrowRecordDto>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBorrowRecord(int id)
        {
            await _borrowRecord.DeleteBorrowRecordAsync(id);
            return NoContent();
        }

        [HttpPost("return/{id}")]
        public async Task<ActionResult<BorrowRecordResponseDto>> ReturnBook(int id)
        {
            var borrowRecord = await _borrowRecord.ReturnBookAsync(id);
            return Ok(borrowRecord);
        }

        [HttpGet("Overdue")]
        public async Task<ActionResult<IEnumerable<BorrowRecordResponseDto>>> GetAllOverdueRecord()
        {
            var borrowRecord = await _borrowRecord.GetAllOverdueRecordAsync();
            return Ok(borrowRecord);
        }

        [HttpGet("Paid")]
        public async Task<ActionResult<IEnumerable<BorrowRecordResponseDto>>> GetAllPaidRecord()
        {
            var borrowRecord = await _borrowRecord.GetAllPaidRecordAsync();
            return Ok(borrowRecord);
        }
    }
}
