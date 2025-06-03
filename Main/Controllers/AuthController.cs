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
        public async Task<IActionResult> Register(RegisterRequestDto dto)
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
        public async Task<IActionResult> VerifyOtp(OtpVerifyRequestDto dto)
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
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            try
            {
                var loginResult = await _authService.LoginAsync(dto);
                //phang token vô cookie httponly
                Response.Cookies.Append(
                    "access_token",
                    loginResult.Token,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true, 
                        SameSite = SameSiteMode.None, 
                        Expires = DateTimeOffset.UtcNow.AddHours(2)
                    });

                return Ok(new { role = loginResult.Role, name = loginResult.Name });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login-google")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] FirebaseLoginRequestDto dto)
        {
            try
            {
                var result = await _authService.FirebaseLoginAsync(dto);

                //phang token vô cookie httponly
                Response.Cookies.Append(
                    "access_token",
                    result.Token,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Expires = DateTimeOffset.UtcNow.AddHours(2)
                    });

                return Ok(new { role = result.Role, name = result.Name });
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
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto dto)
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

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            //lấy token trực tiếp từ cookie
            if (!Request.Cookies.TryGetValue("access_token", out var token) || string.IsNullOrEmpty(token))
                return BadRequest(new { message = "token ko tồn tại." });

            await _authService.LogoutAsync(token);

            //xóa cookie ở client
            Response.Cookies.Delete("access_token", new CookieOptions
            {
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });

            return Ok(new { message = "logout thành công" });
        }
    }
}
