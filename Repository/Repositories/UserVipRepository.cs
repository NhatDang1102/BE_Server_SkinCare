using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repository.Models;

namespace Repository.Repositories
{
    public interface IUserVipRepository
    {
        Task<bool> IsUserVipAsync(Guid userId);
    }

    public class UserVipRepository : IUserVipRepository
    {
        private readonly SkinCareAppContext _db;
        public UserVipRepository(SkinCareAppContext db) { _db = db; }

        public async Task<bool> IsUserVipAsync(Guid userId)
        {
            var now = DateTime.UtcNow;
            return await _db.UserVips.AnyAsync(
                x => x.UserId == userId && x.ExpirationDate > now
            );
        }
    }

}
