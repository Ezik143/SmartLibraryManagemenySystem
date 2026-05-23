using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Interfaces;
using SmartLib.Models.Dto;

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
        public async Task<ActionResult<IEnumerable<BorrowRecordDto>>> GetAllBorrowRecordController()
        {
            var borrowRecord = await _borrowRecord.GetAllBorrowRecordAsync();
            return Ok(borrowRecord);
        }

        // GET api/<BorrowRecordDto>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BorrowRecordDto>> GetBorrowRecordById(int id)
        {
            var borrowRecord = await _borrowRecord.GetBorrowRecordByIdAsync(id);
            return Ok(borrowRecord);
        }

        [HttpGet("fines/{id}")]
        public async Task<ActionResult<BorrowRecordDto>> GetBorrowRecordWithFine(int id)
        {
            var borrowRecord = await _borrowRecord.GetBorrowRecordWithFineAsync(id);
            return Ok(borrowRecord);
        }


        // POST api/<BorrowRecordDto>
        [HttpPost]
        public async Task<ActionResult<BorrowRecordDto>> CreateBorrowRecord(BorrowRecordDto request)
        {
            var borrowRecord = await _borrowRecord.CreateBorrowRecordAsync(request);
            return Ok(borrowRecord);
        }

        // PUT api/<BorrowRecordDto>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<BorrowRecordDto>> UpdateBorrowRecord(int id, BorrowRecordDto request)
        {
            var borrowRecord = await _borrowRecord.UpdateBorrowRecordAsync(id, request);
            return Ok(borrowRecord);
        }

        // DELETE api/<BorrowRecordDto>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBorrowRecord(int id)
        {
            await _borrowRecord.DeleteBorrowRecordAsync(id);
            return NoContent();
        }

        [HttpPost("return/{id}")]
        public async Task<ActionResult<BorrowRecordDto>> ReturnBook(int id)
        {
            var borrowRecord = await _borrowRecord.ReturnBookAsync(id);
            return Ok(borrowRecord);
        }

        [HttpGet("Overdue")]
        public async Task<ActionResult<IEnumerable<BorrowRecordDto>>> GetAllOverdueRecord()
        {
            var borrowRecord = await _borrowRecord.GetAllOverdueRecordAsync();
            return Ok(borrowRecord);
        }

        [HttpGet("Paid")]
        public async Task<ActionResult<IEnumerable<BorrowRecordDto>>> GetAllPaidRecord()
        {
            var borrowRecord = await _borrowRecord.GetAllPaidRecordAsync();
            return Ok(borrowRecord);
        }
    }
}
