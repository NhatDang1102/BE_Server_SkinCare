using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IImageUploadService
    {
        Task<string> UploadAvatarAsync(IFormFile file);
        Task<string> UploadImageProductAsync(IFormFile file);

    }
}
