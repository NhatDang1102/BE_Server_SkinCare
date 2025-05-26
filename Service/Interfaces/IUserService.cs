using Contract.DTOs;
using Repository.DTOs;

namespace Service.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileDto> GetProfileAsync(Guid userId);
        Task UpdateProfileAsync(Guid userId, UpdateUserProfileDto dto);
        Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
        Task UpdateAvatarAsync(Guid userId, string avatarUrl);
    }
}
