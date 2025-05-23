using Microsoft.AspNetCore.Mvc;
using Repository.DTOs;
using Service.Interfaces;

namespace MainApp.Controllers
{
    [ApiController]
    [Route("SkinCare/Auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return Ok(new { message = result });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(OtpVerifyDto dto)
        {
            var result = await _authService.VerifyOtpAsync(dto);
            return Ok(new { message = result });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var loginResult = await _authService.LoginAsync(dto);
                return Ok(loginResult);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login-google")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] FirebaseLoginDto dto)
        {
            var result = await _authService.FirebaseLoginAsync(dto);
            return Ok(result);
        }
    }
}
