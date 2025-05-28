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
using FirebaseAdmin.Auth;
using Contract.DTOs;
using Contract.Helpers;
using Repository.Enums;

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
            //check mail trong ca temp va user
            if (await _repo.EmailExistsAsync(dto.Email))
                return "Email đã tồn tại hoặc chưa xác thực OTP.";
            //gen otp random 6 so
            var otp = new Random().Next(100000, 999999).ToString();
            //check user
            var tempUser = new TempUser
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Name = dto.Name,
                Role = "User",
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
            //check user
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = temp.Email,
                Password = temp.Password,
                Name = temp.Name,
                Role = (UserRole)Enum.Parse(typeof(UserRole), temp.Role),
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _repo.CreateUserAsync(user);
            await _repo.DeleteTempUserAsync(dto.Email);
            //day mail service de gui mail
            await _mailSender.SendWelcomeEmailAsync(user.Email, user.Name);

            return "Xác thực thành công, tài khoản đã được tạo.";
        }

        public async Task<LoginResultDto> LoginAsync(LoginDto dto)
        {
            var user = await _repo.GetUserByEmailAsync(dto.Email);

            //check mail
            if (user == null)
                throw new Exception("Sai mail hoặc mail ko tồn tại.");
            //check status
            if (user.IsActive.HasValue && !user.IsActive.Value)
                throw new Exception("Tài khoản đã bị khóa.");
            //check pass
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                throw new Exception("Pass sai");

            //gen token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name ?? ""),
                new Claim(ClaimTypes.Role, user.Role.ToString())
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
                Role = user.Role.ToString(),
                Name = user.Name
            };
        }

        public async Task<LoginResultDto> FirebaseLoginAsync(FirebaseLoginDto dto)
        {
            // xac minh token
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(dto.IdToken);
            var email = decodedToken.Claims["email"].ToString();

            //check user
            var user = await _repo.GetUserByEmailAsync(email);

            if (user == null)
            {
                user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    Name = decodedToken.Claims.ContainsKey("name") ? decodedToken.Claims["name"].ToString() : "Google User",
                    Role = UserRole.User,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                await _repo.CreateUserAsync(user);
            }

            //gen jwt token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name ?? ""),
                new Claim(ClaimTypes.Role, user.Role.ToString())
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
                Role = user.Role.ToString(),
                Name = user.Name
            };
        }
    }
}
