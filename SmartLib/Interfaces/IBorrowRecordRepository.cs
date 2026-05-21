using Microsoft.AspNetCore.Mvc;
using SmartLib.Models.Dto;

namespace SmartLib.Interfaces
{
    public interface IBorrowRecordRepository
    {
        Task<ActionResult<IEnumerable<BorrowRecordDto>>> GetAllBorrowRecordController();
        Task<ActionResult<BorrowRecordDto>> GetBorrowRecordById(int id);
        Task<ActionResult<BorrowRecordDto>> CreateBorrowRecord(BorrowRecordDto request);
        Task<ActionResult<BorrowRecordDto>> UpdateBorrowRecord(int id, BorrowRecordDto request);
        Task<ActionResult> DeleteBorrowRecord(int id);
    }

}
