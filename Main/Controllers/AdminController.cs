using Contract.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace Main.Controllers
{
    [ApiController]
    [Route("SkinCare/Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminUserController : ControllerBase
    {
        private readonly IAdminService _adminService;
        public AdminUserController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _adminService.GetAllUsersAsync();
                return Ok(users);
            } catch (Exception ex) {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpPut("status")]
        public async Task<IActionResult> UpdateUserStatus([FromBody] UpdateUserStatusDto dto)
        {
            try
            {
                var ok = await _adminService.UpdateUserStatusAsync(dto);
                if (!ok) return NotFound(new { message = "Không tìm thấy user." });
                return Ok(new { message = "Cập nhật trạng thái thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }



        }

        [HttpGet("reg-users-daily")]
        public async Task<IActionResult> CountUsersDaily()
        {
            try
            {
                var count = await _adminService.CountUsersRegisteredDailyAsync();
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });

            }
        }

        [HttpGet("reg-users-weekly")]
        public async Task<IActionResult> CountUsersWeekly()
        {
            try
            {
                var count = await _adminService.CountUsersRegisteredWeeklyAsync();
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });

            }
        }

        [HttpGet("reg-users-monthly")]
        public async Task<IActionResult> CountUsersMonthly()
        {
            try
            {
                var count = await _adminService.CountUsersRegisteredMonthlyAsync();
                return Ok(new { count });

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });

            }
        }
    }
}
