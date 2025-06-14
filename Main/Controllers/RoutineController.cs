using Contract.DTOs;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace MainApp.Controllers
{
    [ApiController]
    [Route("SkinCare/Routine")]
    public class RoutineController : ControllerBase
    {
        private readonly IRoutineService _routineService;

        public RoutineController(IRoutineService routineService)
        {
            _routineService = routineService;
        }

        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> UploadRoutine([FromForm] RoutineRequestDto dto)
        {
            try
            {
                //lay userid
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                             User.FindFirstValue("sub");
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "ko lay duoc userid, xem lai authorize." });

                var result = await _routineService.AnalyzeAndSaveRoutineAsync(Guid.Parse(userId), dto.Image);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyRoutine()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "ko lay duoc userid, xem lai authorize." });

            var result = await _routineService.GetRoutineByUserIdAsync(Guid.Parse(userId));
            if (result == null)
                return NotFound(new { message = "ban chua co routine." });

            return Ok(result);
        }

    }
}
