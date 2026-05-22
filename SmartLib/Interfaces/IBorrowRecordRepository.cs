using Microsoft.AspNetCore.Mvc;
using SmartLib.Models.Dto;
using SmartLib.Models.Entities;

namespace SmartLib.Interfaces
{
    public interface IBorrowRecordRepository
    {
        Task<ActionResult<IEnumerable<BorrowRecordDto>>> GetAllBorrowRecordController();
        Task<ActionResult<BorrowRecordDto>> GetBorrowRecordById(int id);
        Task<ActionResult<BorrowRecordDto>> CreateBorrowRecord(BorrowRecordDto request);
        Task<ActionResult<BorrowRecordDto>> UpdateBorrowRecord(int id, BorrowRecordDto request);
        Task<ActionResult> DeleteBorrowRecord(int id);
        Task<ActionResult<BookDto>> ReturnBook(int Id);
        Task<ActionResult<IEnumerable<BorrowRecordDto>>> GetAllOverdueRecord();
        Task<ActionResult<IEnumerable<BorrowRecordDto>>> GetAllPaidRecord();
    }

}
