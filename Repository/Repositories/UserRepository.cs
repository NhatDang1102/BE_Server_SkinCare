using Repository.Models;
using Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Repository.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SkinCareAppContext _context;

        public UserRepository(SkinCareAppContext context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(Guid id)
            => await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        public async Task<User> GetByEmailAsync(string email)
            => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<UserVip> GetUserVipAsync(Guid userId)
        {
            return await _context.UserVips
                .Where(x => x.UserId == userId && x.ExpirationDate > DateTime.UtcNow)
                .OrderByDescending(x => x.ExpirationDate)
                .FirstOrDefaultAsync();
        }

    }
}
