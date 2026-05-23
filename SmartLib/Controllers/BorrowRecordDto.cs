using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartLib.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowRecordDto : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public BorrowRecordDto(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        // GET: api/<BorrowRecordDto>
        [HttpGet]
        public async Task<IActionResult<BorrowRecordDto>> GetAllBorrowRecord()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<BorrowRecordDto>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<BorrowRecordDto>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<BorrowRecordDto>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<BorrowRecordDto>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
