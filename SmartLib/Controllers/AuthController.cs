using Microsoft.AspNetCore.Mvc;
using SmartLib.Interfaces;
using SmartLib.Models.Dto.Create;
using SmartLib.Models.Dto.Response;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartLib.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApplicationUserResponse>> Login([FromBody] LoginDto request)
        {
            var response = await _authRepository.LoginAsync(request.Email, request.Password);
            return Ok(response);
        }

        [HttpPost("register/student")]
        public async Task<ActionResult<StudentResponseDto>> RegisterStudent([FromBody] CreateStudentDto request)
        {
            var response = await _authRepository.RegisterStudentAsync(request);
            return Ok(response);
        }
    }
}
