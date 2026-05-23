using Microsoft.AspNetCore.Mvc;
using SmartLib.Models.Dto;

namespace SmartLib.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<BookDto>> GetAllBooksAsync();
        Task<BookDto> GetBookByIdAsync(int id);
        Task<BookDto> CreateBookAsync(BookDto request);
        Task<BookDto> UpdateBookAsync(int id, BookDto request);
        Task DeleteBookAsync(int id);
        Task<BookDto> GetBooksByNameAsync(string name);
    }
}
