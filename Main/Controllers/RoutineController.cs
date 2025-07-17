using Contract.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Service.Interfaces;
using Service.Services;

namespace MainApp.Controllers
{
    [ApiController]
    [Route("SkinCare/Routine")]
    public class RoutineController : ControllerBase
    {
        private readonly IRoutineService _routineService;
        private readonly IRoutineFeedbackService _routineFeedbackService;

        public RoutineController(IRoutineService routineService, IRoutineFeedbackService routineFeedbackService)
        {
            _routineService = routineService;
            _routineFeedbackService = routineFeedbackService;
        }

        [Authorize]
        [UserVipRequired]
        [HttpPost("create")]
        public async Task<IActionResult> CreateRoutine([FromForm] CreateRoutineRequestDto dto)
        {
            try
            {
                var userId = GetUserId();
                var result = await _routineService.AnalyzeAndCreateRoutineAsync(userId, dto.Image);
                return Ok(new { message = "Tạo routine thành công!", data = result });
            } catch (DbUpdateException)
            {
                return BadRequest(new { error = "xung đột feedback và routineid" });

            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentRoutine()
        {
            try
            {
                var userId = GetUserId();
                var result = await _routineService.GetRoutineAsync(userId);
                return Ok(new { data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("check")]
        public async Task<IActionResult> CheckRoutineProduct([FromBody] CheckRoutineProductDto dto)
        {
            try
            {
                var userId = GetUserId();
                await _routineService.CheckRoutineProductAsync(dto, userId);
                return Ok(new { message = "Đánh dấu sản phẩm thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("progress/week")]
        public async Task<IActionResult> GetWeeklyProgress()
        {
            try
            {
                var userId = GetUserId();
                var history = await _routineService.GetWeeklyProgressAsync(userId);
                return Ok(new { data = history });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [Authorize]
        [HttpGet("daily")]
        public async Task<IActionResult> GetRoutineDaily([FromQuery] string date = null)
        {
            try
            {
                var userId = GetUserId();
                DateTime? usageDate = null;
                if (!string.IsNullOrEmpty(date))
                {
                    if (!DateTime.TryParse(date, out var d))
                        return BadRequest(new { error = "Sai định dạng ngày (yyyy-MM-dd)" });
                    usageDate = d.Date;
                }
                var result = await _routineService.GetRoutineDailyAsync(userId, usageDate);
                return Ok(new { data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("feedback")]
        public async Task<IActionResult> SubmitFeedback([FromForm] RoutineFeedbackCreateDto dto)
        {
            try
            {
                var userId = GetUserId();
                await _routineFeedbackService.SubmitFeedbackAsync(userId, dto);
                return Ok(new { message = "Đã gửi feedback routine cho admin!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        private Guid GetUserId()
        {
            var userIdStr = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userIdStr == null)
                throw new Exception("Không lấy được user id");
            return Guid.Parse(userIdStr);
        }
    }
}
