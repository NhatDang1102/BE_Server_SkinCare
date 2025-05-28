using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Service.Interfaces;
using Repository.DTOs;

namespace Service.Services
{
    public class ImageUploadService : IImageUploadService
    {
        private readonly Cloudinary _cloudinary;

        //lay key
        public ImageUploadService(IOptions<CloudinarySettings> options)
        {
            var account = new Account(
                options.Value.CloudName,
                options.Value.ApiKey,
                options.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadAvatarAsync(IFormFile file)
        {
            //khai bao stream doc file
            using var stream = file.OpenReadStream();
            //khai bao tt cho cloudinary
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "skincare-avatars"
            };
            //upload len cloudinary
            var result = await _cloudinary.UploadAsync(uploadParams);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
                return result.SecureUrl.ToString();
            throw new Exception("Upload thất bại: " + result.Error?.Message);
        }
    }
}
