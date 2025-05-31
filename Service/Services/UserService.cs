using System;
using System.Threading.Tasks;
using Repository.Interfaces;
using Repository.Models;
using Service.Interfaces;
using BCrypt.Net;
using Repository.DTOs;
using Contract.DTOs;

namespace Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<UserProfileDto> GetProfileAsync(Guid userId)
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user == null) throw new Exception("Ko tim duoc user.");

            return new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                ProfilePicture = user.ProfilePicture
            };
        }

        public async Task UpdateProfileAsync(Guid userId, ProfileRequestDto dto)
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user == null) throw new Exception("Ko tim duoc user.");

            if (!string.IsNullOrEmpty(dto.Name))
                user.Name = dto.Name;

            user.UpdatedAt = DateTime.Now;

            await _repo.UpdateAsync(user);
            await _repo.SaveChangesAsync();
        }

        public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequestDto dto)
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user == null) throw new Exception("Ko tim duoc user.");

            if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.Password))
                throw new Exception("Mật khẩu cũ không đúng.");

            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.Now;

            await _repo.UpdateAsync(user);
            await _repo.SaveChangesAsync();
        }

        public async Task UpdateAvatarAsync(Guid userId, string avatarUrl)
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user == null) throw new Exception("Ko tim duoc user.");

            user.ProfilePicture = avatarUrl;
            user.UpdatedAt = DateTime.Now;

            await _repo.UpdateAsync(user);
            await _repo.SaveChangesAsync();
        }
    }
}
