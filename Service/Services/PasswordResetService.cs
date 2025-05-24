using Repository.Interfaces;
using Repository.Models;
using Service.Helpers;
using Service.Interfaces;

public class PasswordResetService : IPasswordResetService
{
    private readonly IPasswordResetRepository _repo;
    private readonly IAuthRepository _authRepo;
    private readonly MailSender _mailSender;

    public PasswordResetService(IPasswordResetRepository repo, IAuthRepository authRepo, MailSender mailSender)
    {
        _repo = repo;
        _authRepo = authRepo;
        _mailSender = mailSender;
    }

    public async Task SendOtpAsync(string email)
    {
        var user = await _authRepo.GetUserByEmailAsync(email);
        if (user == null) throw new Exception("Không tìm thấy user với email này.");

        //check xem da co request chua
        await _repo.DeleteAllForEmailAsync(email);

        var otp = new Random().Next(100000, 999999).ToString();

        var req = new PasswordResetRequest
        {
            Id = Guid.NewGuid(),
            Email = email,
            Otp = otp,
            OtpExpiration = DateTime.Now.AddMinutes(5),
            CreatedAt = DateTime.Now,
            IsUsed = false
        };
        await _repo.AddAsync(req);
        await _mailSender.SendOtpForgotEmailAsync(email, otp);
    }

    public async Task VerifyOtpAndResetPasswordAsync(string email, string otp, string newPassword)
    {
        var req = await _repo.GetValidByEmailAndOtpAsync(email, otp);
        if (req == null) throw new Exception("OTP sai hoặc đã hết hạn.");

        var user = await _authRepo.GetUserByEmailAsync(email);
        if (user == null) throw new Exception("Không tìm thấy user.");

        user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.UpdatedAt = DateTime.Now;
        await _authRepo.UpdateAsync(user);

        await _repo.InvalidateOtpAsync(req.Id); //tránh reuse otp
    }
}


