using SmartLib.Models.Dto.Create;
using SmartLib.Models.Dto.Response;

namespace SmartLib.Interfaces
{
    public interface IBorrowRecordRepository
    {
        Task<IEnumerable<BorrowRecordResponseDto>> GetAllBorrowRecordAsync();
        Task<BorrowRecordResponseDto> GetBorrowRecordByIdAsync(int id);
        Task<IEnumerable<BorrowRecordResponseDto>> GetBorrowRecordByUserIdAsync(string userId);
        Task<BorrowRecordResponseDto> GetBorrowRecordWithFineAsync(int id);
        Task<BorrowRecordResponseDto> CreateBorrowRecordAsync(CreateBorrowRecordDto request);
        Task<BorrowRecordResponseDto> UpdateBorrowRecordAsync(int id, CreateBorrowRecordDto request);
        Task DeleteBorrowRecordAsync(int id);
        Task<BorrowRecordResponseDto> ReturnBookAsync(int Id);
        Task<IEnumerable<BorrowRecordResponseDto>> GetAllOverdueRecordAsync();
        Task<IEnumerable<BorrowRecordResponseDto>> GetAllPaidRecordAsync();
    }

}
