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
    }
}
