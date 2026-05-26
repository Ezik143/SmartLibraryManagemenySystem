using Microsoft.AspNetCore.Mvc;
using SmartLib.Interfaces;
using SmartLib.Models.Dto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartLib.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {


        private readonly IBookRepository _book;

        public BookController(IBookRepository book)
        {
            _book = book;
        }

        // GET: api/<BookController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetAllBooks()
        {
            var entitiesDto = await _book.GetAllBooksAsync();
            return Ok(entitiesDto);
        }

        // GET api/<BookController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBookById(int id)
        {
            var entityDto = await _book.GetBookByIdAsync(id);
            return Ok(entityDto);
        }

        // POST api/<BookController>
        [HttpPost]
        public async Task<ActionResult<BookDto>> CreateBook(BookDto request)
        {
            var entityDto = await _book.CreateBookAsync(request);
            return NoContent();
        }

        // PUT api/<BookController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<BookDto>> UpdateBook(int id, BookDto request)
        {
            var entityDto = await _book.UpdateBookAsync(id, request);
            return NoContent();
        }

        // DELETE api/<BookController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBook(int id)
        {
            await _book.DeleteBookAsync(id);
            return NoContent();
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<BookDto>> GetBookByName(string name)
        {
            var entityDto = await _book.GetBooksByNameAsync(name);
            return Ok(entityDto);
        }

    }
}
