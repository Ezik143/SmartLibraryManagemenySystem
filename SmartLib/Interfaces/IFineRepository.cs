using SmartLib.Models.Dto.Create;
using SmartLib.Models.Dto.Response;

namespace SmartLib.Interfaces
{
    public interface IFineRepository
    {
        Task<IEnumerable<FineResponseDto>> GetAllFinesAsync();
        Task<FineResponseDto> GetFineByIdAsync(int id);
        Task<FineResponseDto> CreateFineAsync(CreateFineDto request);
        Task<FineResponseDto> UpdateFineAsync(int id, CreateFineDto request);
        Task DeleteFineAsync(int id);
    }
}
