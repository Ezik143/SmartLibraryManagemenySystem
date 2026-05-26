using SmartLib.Models.Dto.Create;
using SmartLib.Models.Dto.Response;

namespace SmartLib.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<BookResponseDto>> GetAllBooksAsync();
        Task<BookResponseDto> GetBookByIdAsync(int id);
        Task<BookResponseDto> CreateBookAsync(CreateBookDto request);
        Task<BookResponseDto> UpdateBookAsync(int id, CreateBookDto request);
        Task DeleteBookAsync(int id);
        Task<BookResponseDto> GetBooksByNameAsync(string name);
    }
}
