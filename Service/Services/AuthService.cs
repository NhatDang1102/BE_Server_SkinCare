using Repository.DTOs;
using Repository.Interfaces;
using Repository.Models;
using Service.Helpers;
using Service.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repo;
        private readonly MailSender _mailSender;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            IAuthRepository repo,
            MailSender mailSender,
            IOptions<JwtSettings> jwtOptions)
        {
            _repo = repo;
            _mailSender = mailSender;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            if (await _repo.EmailExistsAsync(dto.Email))
                return "Email đã tồn tại hoặc chưa xác thực OTP.";

            var otp = new Random().Next(100000, 999999).ToString();

            var tempUser = new TempUser
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Name = dto.Name,
                Role = dto.Role ?? "User",
                Otp = otp,
                Otpexpiration = DateTime.Now.AddMinutes(5),
                CreatedAt = DateTime.Now
            };

            await _repo.AddTempUserAsync(tempUser);
            await _mailSender.SendOtpEmailAsync(dto.Email, otp);

            return "Đã gửi OTP đến email, vui lòng xác minh.";
        }

        public async Task<string> VerifyOtpAsync(OtpVerifyDto dto)
        {
            var temp = await _repo.GetTempUserByEmailAsync(dto.Email);
            if (temp == null) return "Email không tồn tại hoặc đã tồn tại.";
            if (temp.Otp != dto.Otp) return "OTP sai.";
            if (temp.Otpexpiration < DateTime.Now) return "OTP đã hết hạn.";

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = temp.Email,
                Password = temp.Password,
                Name = temp.Name,
                Role = temp.Role,
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _repo.CreateUserAsync(user);
            await _repo.DeleteTempUserAsync(dto.Email);

            await _mailSender.SendWelcomeEmailAsync(user.Email, user.Name);

            return "Xác thực thành công, tài khoản đã được tạo.";
        }

        public async Task<LoginResultDto> LoginAsync(LoginDto dto)
        {
            var user = await _repo.GetUserByEmailAsync(dto.Email);

            if (user == null || (user.IsActive.HasValue && !user.IsActive.Value))
                throw new Exception("Sai pass/mail.");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                throw new Exception("Pass sai");

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name ?? ""),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new LoginResultDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Role = user.Role,
                Name = user.Name
            };
        }
    }
}
