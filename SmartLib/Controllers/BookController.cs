using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLib.Data;
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

        BookController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/<BookController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> Get()
        {
            var books = await _context.Books.ToListAsync();
            var bookDto = _mapper.Map<IEnumerable<BookDto>>(books);
            return Ok(bookDto);
        }

        // GET api/<BookController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> Get(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            var bookDto = _mapper.Map<BookDto>(book);
            return Ok(bookDto);
        }

        // POST api/<BookController>
        [HttpPost]
        public async Task<ActionResult<BookDto>> CreateBook(int id, BookDto request)
        {
            var result = await _context.Books.FindAsync(id);

            if (result == null)
            {
                return NotFound($"{id} not found.");
            }

            var book = _mapper.Map<Book>(result);
            await _context.AddAsync(book);
            await _context.SaveChangesAsync();

            var response = _mapper.Map<BookDto>(book);
            return Ok(response);
        }

        // PUT api/<BookController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<BookController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
