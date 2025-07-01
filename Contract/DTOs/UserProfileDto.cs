namespace Contract.DTOs
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }   
        public string Name { get; set; }
        public string ProfilePicture { get; set; }
        public DateTime? VipExpirationDate { get; set; }
    }
}
