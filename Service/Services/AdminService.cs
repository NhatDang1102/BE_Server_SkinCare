using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.DTOs;
using Repository.Interfaces;
using Service.Interfaces;

namespace Service.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _repo;
        public AdminService(IAdminRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<UserSimpleDto>> GetAllUsersAsync()
        {
            var users = await _repo.GetAllUsersAsync();
            return users.Select(u => new UserSimpleDto
            {
                Id = u.Id,
                Email = u.Email,
                Name = u.Name,
                Role = u.Role.ToString(),
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            }).ToList();
        }

        public async Task<bool> UpdateUserStatusAsync(UpdateUserStatusDto dto)
        {
            var user = await _repo.GetUserByIdAsync(dto.UserId);
            if (user == null) return false;
            user.IsActive = dto.IsActive;
            await _repo.UpdateUserAsync(user);
            return true;
        }

        public async Task<int> CountUsersRegisteredDailyAsync()
        {
            var result = await _repo.CountUsersRegisteredDailyAsync();
            return result;
        }

        public async Task<int> CountUsersRegisteredWeeklyAsync()
        {
            var result = await _repo.CountUsersRegisteredWeeklyAsync();
            return result;
        }

        public async Task<int> CountUsersRegisteredMonthlyAsync()
        {
            var result = await _repo.CountUsersRegisteredMonthlyAsync();
            return result;
        }
    }
}
