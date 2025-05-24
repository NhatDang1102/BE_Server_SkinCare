using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.DTOs;
using Service.Interfaces;

namespace MainApp.Controllers
{
    [ApiController]
    [Route("SkinCare/Profile")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IImageUploadService _imageUploadService;

        public ProfileController(IUserService userService, IImageUploadService imageUploadService)
        {
            _userService = userService;
            _imageUploadService = imageUploadService;
        }

        private Guid GetUserId()
        {
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(idStr);
        }


        [HttpGet]
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            var profile = await _userService.GetProfileAsync(GetUserId());
            return Ok(profile);
        }

       //update name
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto dto)
        {
            await _userService.UpdateProfileAsync(GetUserId(), dto);
            return Ok(new { message = "Update thanh cong" });
        }

        // doi mk
        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            await _userService.ChangePasswordAsync(GetUserId(), dto);
            return Ok(new { message = "Update thanh cong" });
        }

        // Upload avatar (file multiform)

        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar([FromForm] UploadAvatarDto dto)
        {
            if (dto.ImageFile == null || dto.ImageFile.Length == 0)
                return BadRequest("File ko hop le.");

            var avatarUrl = await _imageUploadService.UploadAvatarAsync(dto.ImageFile);
            await _userService.UpdateAvatarAsync(GetUserId(), avatarUrl);
            return Ok(new { url = avatarUrl });
        }
    }
}
