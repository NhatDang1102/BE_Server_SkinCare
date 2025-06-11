using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract.DTOs;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Repository.Models;

namespace Repository.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly SkinCareAppContext _context;
        public AdminRepository(SkinCareAppContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllUsersAsync() => await _context.Users.ToListAsync();
        public async Task<User> GetUserByIdAsync(Guid id) => await _context.Users.FindAsync(id);
        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        //ca 3 api sau deu lay theo created at het, chu nhat se tinh la 0
        public async Task<int> CountUsersRegisteredDailyAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _context.Users
                .CountAsync(u => u.CreatedAt.HasValue && u.CreatedAt.Value.Date == today);
        }

        public async Task<int> CountUsersRegisteredWeeklyAsync()
        {
            var today = DateTime.UtcNow.Date;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            return await _context.Users
                .CountAsync(u => u.CreatedAt.HasValue
                            && u.CreatedAt.Value.Date >= startOfWeek
                            && u.CreatedAt.Value.Date <= today);
        }

        public async Task<int> CountUsersRegisteredMonthlyAsync()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            return await _context.Users
                .CountAsync(u => u.CreatedAt.HasValue
                            && u.CreatedAt.Value >= startOfMonth
                            && u.CreatedAt.Value <= now);
        }
    }
}
