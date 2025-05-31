using Contract.DTOs;
using Repository.DTOs;

namespace Service.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileDto> GetProfileAsync(Guid userId);
        Task UpdateProfileAsync(Guid userId, ProfileRequestDto dto);
        Task ChangePasswordAsync(Guid userId, ChangePasswordRequestDto dto);
        Task UpdateAvatarAsync(Guid userId, string avatarUrl);
    }
}
