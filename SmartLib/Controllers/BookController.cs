using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Interfaces;
using SmartLib.Models.Dto.Create;
using SmartLib.Models.Dto.Response;

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
        public async Task<ActionResult<IEnumerable<BookResponseDto>>> GetAllBooks()
        {
            var entitiesDto = await _book.GetAllBooksAsync();
            return Ok(entitiesDto);
        }

        // GET api/<BookController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookResponseDto>> GetBookById(int id)
        {
            var entityDto = await _book.GetBookByIdAsync(id);
            return Ok(entityDto);
        }

        // POST api/<BookController>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<BookResponseDto>> CreateBook(CreateBookDto request)
        {
            var entityDto = await _book.CreateBookAsync(request);
            return NoContent();
        }

        // PUT api/<BookController>/
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<BookResponseDto>> UpdateBook(int id, CreateBookDto request)
        {
            var entityDto = await _book.UpdateBookAsync(id, request);
            return NoContent();
        }

        // DELETE api/<BookController>/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBook(int id)
        {
            await _book.DeleteBookAsync(id);
            return NoContent();
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<BookResponseDto>> GetBookByName(string name)
        {
            var entityDto = await _book.GetBooksByNameAsync(name);
            return Ok(entityDto);
        }

    }
}
