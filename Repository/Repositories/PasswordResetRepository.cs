using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Repository.Models;

namespace Repository.Repositories
{
    public class PasswordResetRepository : IPasswordResetRepository
    {
        private readonly SkinCareAppContext _context;
        public PasswordResetRepository(SkinCareAppContext context) => _context = context;

        public async Task AddAsync(PasswordResetRequest req)
        {
            await _context.PasswordResetRequests.AddAsync(req);
            await _context.SaveChangesAsync();
        }

        public async Task<PasswordResetRequest> GetValidByEmailAndOtpAsync(string email, string otp)
        {
            return await _context.PasswordResetRequests.FirstOrDefaultAsync(x => x.Email == email && x.Otp == otp && x.OtpExpiration > DateTime.Now && !x.IsUsed);
        }

        public async Task InvalidateOtpAsync(Guid id)
        {
            var req = await _context.PasswordResetRequests.FindAsync(id);
            if (req != null)
            {
                req.IsUsed = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAllForEmailAsync(string email)
        {
            var reqs = _context.PasswordResetRequests.Where(x => x.Email == email);
            _context.PasswordResetRequests.RemoveRange(reqs);
            await _context.SaveChangesAsync();
        }
    }

}
