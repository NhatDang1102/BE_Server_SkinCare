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

        public async Task<List<PaymentLogDto>> GetPaymentLogsAsync()
        {
            return await _context.PaymentLogs
                .Include(x => x.User)
                .Include(x => x.Vippackage)
                .OrderByDescending(x => x.PaymentDate)
                .Select(x => new PaymentLogDto
                {
                    UserEmail = x.User.Email,
                    PackageName = x.Vippackage.Name,
                    PaymentAmount = x.PaymentAmount,
                    PaymentStatus = x.PaymentStatus,
                    PaymentDate = x.PaymentDate
                }).ToListAsync();
        }

        public async Task<List<UserSimpleDto>> GetAllUsersWithVipAsync()
        {
            var now = DateTime.UtcNow;
            var query = from u in _context.Users
                        join v in _context.UserVips on u.Id equals v.UserId into g
                        from vip in g.OrderByDescending(x => x.ExpirationDate).Take(1).DefaultIfEmpty()
                        select new UserSimpleDto
                        {
                            Id = u.Id,
                            Email = u.Email,
                            Name = u.Name,
                            Role = u.Role.ToString(),
                            IsActive = u.IsActive,
                            CreatedAt = u.CreatedAt,
                            VipExpirationDate = vip.ExpirationDate
                        };
            return await query.ToListAsync();
        }

        public async Task<List<PaymentLogDto>> GetPaymentLogsByDateRangeAsync(DateTime start, DateTime end)
        {
            return await _context.PaymentLogs
                .Include(x => x.User)
                .Where(x => x.PaymentDate >= start && x.PaymentDate < end)
                .OrderBy(x => x.PaymentDate)
                .Select(x => new PaymentLogDto
                {
                    UserEmail = x.User.Email,
                    PackageName = x.Vippackage.Name,
                    PaymentAmount = x.PaymentAmount,
                    PaymentStatus = x.PaymentStatus,
                    PaymentDate = x.PaymentDate
                }).ToListAsync();
        }

    }
}

