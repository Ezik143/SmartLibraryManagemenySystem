using Microsoft.AspNetCore.Mvc;
using SmartLib.Models.Dto;
using SmartLib.Models.Entities;

namespace SmartLib.Interfaces
{
    public interface IBorrowRecordRepository
    {
        Task<IEnumerable<BorrowRecordDto>> GetAllBorrowRecordAsync();
        Task<BorrowRecordDto> GetBorrowRecordByIdAsync(int id);
        Task<BorrowRecordDto> GetBorrowRecordWithFineAsync(int id);
        Task<BorrowRecordDto> CreateBorrowRecordAsync(BorrowRecordDto request);
        Task<BorrowRecordDto> UpdateBorrowRecordAsync(int id, BorrowRecordDto request);
        Task DeleteBorrowRecordAsync(int id);
        Task<BorrowRecordDto> ReturnBookAsync(int Id);
        Task<IEnumerable<BorrowRecordDto>> GetAllOverdueRecordAsync();
        Task<IEnumerable<BorrowRecordDto>> GetAllPaidRecordAsync();
    }

}
