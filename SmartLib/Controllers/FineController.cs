using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SmartLib.Data;
using SmartLib.Interfaces;
using SmartLib.Models.Dto;
using SmartLib.Models.Entities;

// FineController - Handles HTTP requests for fine management operations (CRUD)

namespace SmartLib.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FineController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFineRepository _fine;

        public FineController(ApplicationDbContext context, IMapper mapper, IFineRepository fine)
        {
            _context = context;
            _mapper = mapper;
            _fine = fine;
        }

        // GET: api/<FineController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FineDto>>> GetAllFines()
        {
            var entitiesDto = await _fine.GetAllFinesAsync();
            return Ok(entitiesDto);
        }

        // GET api/<FineController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FineDto>> GetFineById(int id)
        {
            var entityDto = await _fine.GetFineByIdAsync(id);
            return Ok(entityDto);
        }

        // POST api/<FineController>
        [HttpPost]
        public async Task<ActionResult<FineDto>> CreateFine(FineDto request)
        {
            var entityDto = await _fine.CreateFineAsync(request);
            return Ok(entityDto);
        }

        // PUT api/<FineController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<FineDto>> UpdateFine(int id, FineDto request)
        {
            var entityDto = await _fine.UpdateFineAsync(id, request);
            return Ok(entityDto);
        }

        // DELETE api/<FineController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFine(int id)
        {
            await _fine.DeleteFineAsync(id);
            return Ok();
        }
    }
}
