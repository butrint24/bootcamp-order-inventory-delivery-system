using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Shared.DTOs;
using Application.Services.Interfaces;


namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("signup")]
        public async Task<ActionResult<AuthResponseDto>> Signup([FromBody] SignupRequestDto dto)
        {
            try
            {
                var result = await _authService.SignupAsync(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto dto)
        {
            try
            {
                var result = await _authService.LoginAsync(dto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid credentials.");
            }
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<TokenResponseDto>> Refresh([FromBody] RefreshRequestDto dto)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(dto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
