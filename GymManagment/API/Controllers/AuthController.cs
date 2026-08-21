using GymManagment.Application.Interfaces;
using GymManagment.Domain.Common;
using GymManagment.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;


namespace GymManagment.API.Controllers
{
    [ApiController]

    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]

        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto dto)
        {

            Result result = await _authService.RegisterAsync(dto);

            if (result.ErrorType == Result.ErrorTypes.Conflict) return Conflict(result.ErrorMessage);

            if (result.ErrorType == Result.ErrorTypes.ServerError) return StatusCode(500, result.ErrorMessage);

            if (result.ErrorType == Result.ErrorTypes.ValidationError) return BadRequest(result.ErrorMessage);

            return Ok();
            }
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto dto)
        {
            var login = await _authService.LoginAsync(dto);
            if (login == null) return Unauthorized("Неверный логин или пароль");
            return Ok(login);

        }
        [HttpPost("refresh")]
        
        public async Task<IActionResult> RefreshAsync([FromBody] RefreshRequestDto dto)
        {
            var refresh = await _authService.RefreshTokenAsync(dto);
            if (refresh == null) return Unauthorized("Невалидный или истёкший refresh-токен");
            return Ok(refresh);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync([FromBody] RefreshRequestDto dto)
        {
            var result = await _authService.LogoutAsync(dto);
            if (result.ErrorType ==Result.ErrorTypes.NotFound) return NotFound(result.ErrorMessage);
            return Ok();
        }
    }
}
