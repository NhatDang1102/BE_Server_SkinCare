using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Models;

namespace Repository.Interfaces
{
    public interface IPasswordResetRepository
    {
        Task AddAsync(PasswordResetRequest req);
        Task<PasswordResetRequest> GetValidByEmailAndOtpAsync(string email, string otp);
        Task InvalidateOtpAsync(Guid id);
        Task DeleteAllForEmailAsync(string email);
    }

}
