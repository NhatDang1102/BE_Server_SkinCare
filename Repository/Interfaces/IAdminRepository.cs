using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.DTOs;
using Repository.Models;

namespace Repository.Interfaces
{
    public interface IAdminRepository
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(Guid id);
        Task UpdateUserAsync(User user);
        Task<int> CountUsersRegisteredDailyAsync();
        Task<int> CountUsersRegisteredWeeklyAsync();
        Task<int> CountUsersRegisteredMonthlyAsync();
    }
}
