using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLib.Data;
using SmartLib.Interfaces;
using SmartLib.Models.Dto;
using SmartLib.Models.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartLib.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        private readonly IBookRepository _book;

        public BookController(ApplicationDbContext context, IMapper mapper, IBookRepository book)
        {
            _context = context;
            _mapper = mapper;
            _book = book;
        }

        // GET: api/<BookController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetAllBooks()
        {
            var entities = await _context.Books.ToListAsync();
            var dtos = _mapper.Map<IEnumerable<BookDto>>(entities);
            return Ok(dtos);
        }

        // GET api/<BookController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBookById(int id)
        {
            var entity = await _context.Books.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            var dto = _mapper.Map<BookDto>(entity);
            return Ok(dto);
        }

        // POST api/<BookController>
        [HttpPost]
        public async Task<ActionResult<BookDto>> CreateBook(BookDto request)
        {
            if (request == null)
            {
                return BadRequest("Book data is required.");
            }

            var result = await _context.Books.FirstOrDefaultAsync(b => b.Isbn == request.Isbn);

            if (result != null)
            {
                return Conflict($"The isbn{request.Isbn} already exist.");
            }

            var entity = _mapper.Map<Book>(request);

            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<BookDto>(entity);
            return Ok(dto);
        }

        // PUT api/<BookController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<BookDto>> UpdateBook(int id, BookDto request)
        {
            if (request == null)
            {
                return BadRequest("Book data is required.");
            }

            var entity = await _context.Books.FindAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            _mapper.Map(request, entity);
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<BookDto>(entity);
            return Ok(dto);
        }

        // DELETE api/<BookController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBook(int id)
        {
            var entity = await _context.Books.FindAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<BookDto>> GetBookByName(string name)
        {
            var entities = await _context.Books.FirstOrDefaultAsync(b => b.Title == name);

            if (entities == null)
            {
                return NotFound();
            }

            var dto = _mapper.Map<BookDto>(entities);
            return Ok(dto);
        }

    }
}
