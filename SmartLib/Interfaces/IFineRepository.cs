using SmartLib.Models.Dto;

namespace SmartLib.Interfaces
{
    public interface IFineRepository
    {
        Task<IEnumerable<FineDto>> GetAllFinesAsync();
        Task<FineDto> GetFineByIdAsync(int id);
        Task<FineDto> CreateFineAsync(FineDto request);
        Task<FineDto> UpdateFineAsync(int id, FineDto request);
        Task DeleteFineAsync(int id);
    }
}
