namespace Contract.DTOs
{
    public class RegisterRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
    }

    public class OtpVerifyRequestDto
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }
}
