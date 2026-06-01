using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Interfaces;
using SmartLib.Models.Dto.Create;
using SmartLib.Models.Dto.Response;
using System.Security.Claims;

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
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BorrowRecordResponseDto>>> GetAllBorrowRecordController()
        {
            var borrowRecord = await _borrowRecord.GetAllBorrowRecordAsync();
            return Ok(borrowRecord);
        }

        // GET api/<BorrowRecordDto>/5
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<BorrowRecordResponseDto>> GetBorrowRecordById(int id)
        {
            var borrowRecord = await _borrowRecord.GetBorrowRecordByIdAsync(id);
            return Ok(borrowRecord);
        }


        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<BorrowRecordResponseDto>>> GetBorrowRecordByUserId(string userId)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin && UserId != userId)
            {
                return Problem(
    detail: "You do not have permission to view this record.",
    statusCode: 403
);
            }

            var borrowRecord = await _borrowRecord.GetBorrowRecordByUserIdAsync(userId);
            return Ok(borrowRecord);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("fines/{id}")]
        public async Task<ActionResult<BorrowRecordResponseDto>> GetBorrowRecordWithFine(int id)
        {
            var borrowRecord = await _borrowRecord.GetBorrowRecordWithFineAsync(id);
            return Ok(borrowRecord);
        }



        // POST api/<BorrowRecordDto>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<BorrowRecordResponseDto>> CreateBorrowRecord(CreateBorrowRecordDto request)
        {
            var borrowRecord = await _borrowRecord.CreateBorrowRecordAsync(request);
            return NoContent();
        }

        // PUT api/<BorrowRecordDto>/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<BorrowRecordResponseDto>> UpdateBorrowRecord(int id, CreateBorrowRecordDto request)
        {
            var borrowRecord = await _borrowRecord.UpdateBorrowRecordAsync(id, request);
            return NoContent();
        }

        // DELETE api/<BorrowRecordDto>/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBorrowRecord(int id)
        {
            await _borrowRecord.DeleteBorrowRecordAsync(id);
            return NoContent();
        }

        [HttpPost("return/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BorrowRecordResponseDto>> ReturnBook(int id)
        {
            var borrowRecord = await _borrowRecord.ReturnBookAsync(id);
            return Ok(borrowRecord);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("Overdue")]
        public async Task<ActionResult<IEnumerable<BorrowRecordResponseDto>>> GetAllOverdueRecord()
        {
            var borrowRecord = await _borrowRecord.GetAllOverdueRecordAsync();
            return Ok(borrowRecord);
        }

        [HttpGet("Paid")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<BorrowRecordResponseDto>>> GetAllPaidRecord()
        {
            var borrowRecord = await _borrowRecord.GetAllPaidRecordAsync();
            return Ok(borrowRecord);
        }
    }
}
