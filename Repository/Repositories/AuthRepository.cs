using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Repository.Models;

namespace Repository.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly SkinCareAppContext _context;

        public AuthRepository(SkinCareAppContext context)
        {
            _context = context;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email)
                || await _context.TempUsers.AnyAsync(u => u.Email == email);
        }

        public async Task AddTempUserAsync(TempUser tempUser)
        {
            await _context.TempUsers.AddAsync(tempUser);
            await _context.SaveChangesAsync();
        }

        public async Task<TempUser> GetTempUserByEmailAsync(string email)
        {
            return await _context.TempUsers.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task DeleteTempUserAsync(string email)
        {
            var entity = await GetTempUserByEmailAsync(email);
            if (entity != null)
            {
                _context.TempUsers.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
