using Microsoft.AspNetCore.Mvc;
using SmartLib.Models.Dto;

namespace SmartLib.Interfaces
{
    public interface IBookRepository
    {
        Task<ActionResult<IEnumerable<BookDto>>> GetAllBooks();
        Task<ActionResult<BookDto>> GetBookById(int id);
        Task<ActionResult<BookDto>> CreateBook(BookDto request);
        Task<ActionResult<BookDto>> UpdateBook(int id, BookDto request);
        Task<ActionResult> DeleteBook(int id);
        Task<ActionResult<IEnumerable<BookDto>>> GetBooksByName(string name);
    }
}
