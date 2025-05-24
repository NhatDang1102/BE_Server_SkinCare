using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Service.Helpers
{
    public class MailSender
    {
        private readonly SmtpSettings _smtp;
        public MailSender(IOptions<SmtpSettings> smtpOptions)
        {
            _smtp = smtpOptions.Value;
        }

        public async Task SendOtpEmailAsync(string toEmail, string otp)
        {
            var mail = new MailMessage(_smtp.FromEmail, toEmail)
            {
                Subject = "Xác nhận đăng ký tài khoản - SkinCareApp",
                Body = $"Mã OTP của bạn là: {otp}\nMã sẽ hết hạn sau 5 phút.",
                IsBodyHtml = false
            };

            using var smtpClient = new SmtpClient(_smtp.Host, _smtp.Port)
            {
                EnableSsl = _smtp.EnableSsl,
                Credentials = new NetworkCredential(_smtp.FromEmail, _smtp.AppPassword)
            };
            await smtpClient.SendMailAsync(mail);
        }

        public async Task SendOtpForgotEmailAsync(string toEmail, string otp)
        {
            var mail = new MailMessage(_smtp.FromEmail, toEmail)
            {
                Subject = "Yêu cầu đặt lại mật khẩu - SkinCareApp",
                Body = $"Mã OTP đặt lại mật khẩu của bạn là: {otp}\nMã sẽ hết hạn sau 5 phút.",
                IsBodyHtml = false
            };

            using var smtpClient = new SmtpClient(_smtp.Host, _smtp.Port)
            {
                EnableSsl = _smtp.EnableSsl,
                Credentials = new NetworkCredential(_smtp.FromEmail, _smtp.AppPassword)
            };
            await smtpClient.SendMailAsync(mail);
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string name)
        {
            var mail = new MailMessage(_smtp.FromEmail, toEmail)
            {
                Subject = "Chào mừng đến với SkinCareApp!",
                Body = $"Xin chào {name},\nCảm ơn bạn đã đăng ký thành công tài khoản tại SkinCareApp.",
                IsBodyHtml = false
            };

            using var smtpClient = new SmtpClient(_smtp.Host, _smtp.Port)
            {
                EnableSsl = _smtp.EnableSsl,
                Credentials = new NetworkCredential(_smtp.FromEmail, _smtp.AppPassword)
            };
            await smtpClient.SendMailAsync(mail);
        }
    }
}
