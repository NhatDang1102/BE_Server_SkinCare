using Contract.DTOs;
using Microsoft.AspNetCore.Mvc;
using Repository.DTOs;
using Service.Interfaces;
using Service.Services;

namespace MainApp.Controllers
{
    [ApiController]
    [Route("SkinCare/Auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IPasswordResetService _resetService;

        public AuthController(IAuthService authService, IPasswordResetService resetService)
        {
            _authService = authService;
            _resetService = resetService;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            try
            {
                var result = await _authService.RegisterAsync(dto);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(OtpVerifyDto dto)
        {
            try
            {
                var result = await _authService.VerifyOtpAsync(dto);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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
            try
            {
                var result = await _authService.FirebaseLoginAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
        {
            try
            {
                await _resetService.SendOtpAsync(dto.Email);
                return Ok(new { message = "Đã gửi OTP, vui lòng kiểm tra email." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("verify-forgot-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {


            try
            {
                await _resetService.VerifyOtpAndResetPasswordAsync(dto.Email, dto.Otp, dto.NewPassword);
                return Ok(new { message = "Đặt lại mật khẩu thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
