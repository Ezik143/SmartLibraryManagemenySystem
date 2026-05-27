using Microsoft.AspNetCore.Mvc;
using SmartLib.Interfaces;
using SmartLib.Models.Dto.Create;
using SmartLib.Models.Dto.Response;
// FineController - Handles HTTP requests for fine management operations (CRUD)

namespace SmartLib.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FineController : ControllerBase
    {

        private readonly IFineRepository _fine;

        public FineController(IFineRepository fine)
        {
            _fine = fine;
        }

        // GET: api/<FineController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FineResponseDto>>> GetAllFines()
        {
            var entitiesDto = await _fine.GetAllFinesAsync();
            return Ok(entitiesDto);
        }

        // GET api/<FineController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FineResponseDto>> GetFineById(int id)
        {
            var entityDto = await _fine.GetFineByIdAsync(id);
            return Ok(entityDto);
        }

        // POST api/<FineController>
        [HttpPost]
        public async Task<ActionResult<FineResponseDto>> CreateFine(CreateFineDto request)
        {
            var entityDto = await _fine.CreateFineAsync(request);
            return NoContent();
        }

        // PUT api/<FineController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<FineResponseDto>> UpdateFine(int id, CreateFineDto request)
        {
            var entityDto = await _fine.UpdateFineAsync(id, request);
            return NoContent();
        }

        // DELETE api/<FineController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFine(int id)
        {
            await _fine.DeleteFineAsync(id);
            return NoContent();
        }
    }
}
