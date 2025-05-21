namespace Service.Helpers
{
    public class SmtpSettings
    {
        public string FromEmail { get; set; }
        public string AppPassword { get; set; }
        public string Host { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;
    }
}
