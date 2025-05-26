using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IPasswordResetService
    {
        Task SendOtpAsync(string email);
        Task VerifyOtpAndResetPasswordAsync(string email, string otp, string newPassword);
    }

}
