using System.Threading.Tasks;
using Contract.DTOs;
using Repository.DTOs;

namespace Service.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterRequestDto dto);
        Task<string> VerifyOtpAsync(OtpVerifyRequestDto dto);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
        Task<LoginResponseDto> FirebaseLoginAsync(FirebaseLoginRequestDto dto);
        Task LogoutAsync(string token);
    }
}
