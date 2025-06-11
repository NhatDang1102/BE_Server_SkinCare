using System.Security.Claims;
using Contract.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.DTOs;
using Service.Interfaces;
using Service.Services;
using StackExchange.Redis;

namespace MainApp.Controllers
{
    [ApiController]
    [Route("SkinCare/Profile")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IImageUploadService _imageUploadService;
        private readonly IRedisService _redis;

        public ProfileController(IUserService userService, IImageUploadService imageUploadService, IRedisService redis)
        {
            _userService = userService;
            _imageUploadService = imageUploadService;
            _redis = redis;

        }

        private Guid GetUserId()
        {
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(idStr);
        }


        [HttpGet]
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            try
            {
                var profile = await _userService.GetProfileAsync(GetUserId());
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

       //update name
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileRequestDto dto)
        {
            try
            {
                await _userService.UpdateProfileAsync(GetUserId(), dto);
                return Ok(new { message = "Update thanh cong" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
 
        }

        // doi mk
        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto dto)
        {
            try
            {
                await _userService.ChangePasswordAsync(GetUserId(), dto);
                return Ok(new { message = "Update thanh cong" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        // Upload avatar (file multiform)

        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar([FromForm] UploadAvatarDto dto)
        {
            try
            {
                if (dto.ImageFile == null || dto.ImageFile.Length == 0)
                    return BadRequest("File ko hop le.");

                var avatarUrl = await _imageUploadService.UploadAvatarAsync(dto.ImageFile);
                await _userService.UpdateAvatarAsync(GetUserId(), avatarUrl);
                return Ok(new { url = avatarUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpGet("login-history")]
        public async Task<IActionResult> GetLoginHistory()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var history = await _redis.GetLoginHistoryAsync(userId); // không truyền take
                return Ok(history);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
    }
}

