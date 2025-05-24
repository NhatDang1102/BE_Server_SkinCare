using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IImageUploadService
    {
        Task<string> UploadAvatarAsync(IFormFile file);
    }
}
