using System.Threading.Tasks;
using Repository.DTOs;

namespace Service.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto dto);
        Task<string> VerifyOtpAsync(OtpVerifyDto dto);
        Task<LoginResultDto> LoginAsync(LoginDto dto);
    }
}
