using Microsoft.AspNetCore.Http;

namespace Contract.DTOs
{
    public class ProfileRequestDto
    {
        public string Name { get; set; }
    }

    public class UpdateUserStatusDto
    {
        public Guid UserId { get; set; }
        public bool IsActive { get; set; }
    }

    public class UploadAvatarDto
    {
        public IFormFile ImageFile { get; set; }
    }
}
