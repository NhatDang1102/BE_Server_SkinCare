using Contract.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Services;

namespace Main.Controllers
{
    [ApiController]
    [Route("SkinCare/Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminUserController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IRoutineFeedbackService _routineFeedbackService;
        public AdminUserController(IAdminService adminService, IRoutineFeedbackService routineFeedbackService)
        {
            _adminService = adminService;
            _routineFeedbackService = routineFeedbackService;
        }

        [HttpGet("users/get-all-users")]
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

        [HttpPut("users/status")]
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
        [HttpGet("users/reg-users-daily")]
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

        [HttpGet("users/reg-users-weekly")]
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

        [HttpGet("users/reg-users-monthly")]
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

        [HttpGet("users/count-login-daily")]
        public async Task<IActionResult> CountUserLoginDaily()
        {
            try
            {
                var count = await _adminService.CountUserLoggedInDailyAsync();
                return Ok(new { count });
            } catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });

            }

        }
        [HttpGet("revenue/all-excel")]
        public async Task<IActionResult> ExportPaymentLogExcel()
        {
            var logs = await _adminService.GetPaymentLogsAsync();
            var bytes = ExcelHelper.ExportPaymentLogs(logs);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "payment-log.xlsx");
        }

        [HttpGet("users/excel")]
        public async Task<IActionResult> ExportUserListExcel()
        {
            var users = await _adminService.GetAllUsersWithVipAsync();
            var bytes = ExcelHelper.ExportUsers(users);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "users.xlsx");
        }

        [HttpGet("revenue/daily/excel")]
        public async Task<IActionResult> ExportDailyRevenueExcel()
        {
            var today = DateTime.UtcNow.Date;
            var logs = await _adminService.GetPaymentLogsByDateRangeAsync(today, today.AddDays(1));
            var bytes = ExcelHelper.ExportRevenue(logs, $"Doanh thu ngày {today:dd/MM/yyyy}");
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "revenue-daily.xlsx");
        }

        [HttpGet("revenue/weekly/excel")]
        public async Task<IActionResult> ExportWeeklyRevenueExcel()
        {
            var today = DateTime.UtcNow.Date;
            var monday = today.AddDays(-(int)today.DayOfWeek + 1);
            var sunday = monday.AddDays(6);
            var logs = await _adminService.GetPaymentLogsByDateRangeAsync(monday, sunday.AddDays(1));
            var bytes = ExcelHelper.ExportRevenue(logs, $"Doanh thu tuần {monday:dd/MM} - {sunday:dd/MM/yyyy}");
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "revenue-weekly.xlsx");
        }

        [HttpGet("revenue/monthly/excel")]
        public async Task<IActionResult> ExportMonthlyRevenueExcel()
        {
            var now = DateTime.UtcNow;
            var start = new DateTime(now.Year, now.Month, 1);
            var end = start.AddMonths(1);
            var logs = await _adminService.GetPaymentLogsByDateRangeAsync(start, end);
            var bytes = ExcelHelper.ExportRevenue(logs, $"Doanh thu tháng {now:MM/yyyy}");
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "revenue-monthly.xlsx");
        }

        [HttpGet("revenue/daily")]
        public async Task<IActionResult> GetDailyRevenue()
        {
            var today = DateTime.UtcNow.Date;
            var logs = await _adminService.GetPaymentLogsByDateRangeAsync(today, today.AddDays(1));

            var total = logs
    .Where(x => x.PaymentStatus == "Completed")
    .Sum(x => x.PaymentAmount ?? 0);
            return Ok(new { total, logs });
        }

        [HttpGet("revenue/weekly")]
        public async Task<IActionResult> GetWeeklyRevenue()
        {
            var today = DateTime.UtcNow.Date;
            var monday = today.AddDays(-(int)today.DayOfWeek + 1);
            var sunday = monday.AddDays(6);
            var logs = await _adminService.GetPaymentLogsByDateRangeAsync(monday, sunday.AddDays(1));
            var total = logs
    .Where(x => x.PaymentStatus == "Completed")
    .Sum(x => x.PaymentAmount ?? 0);
            return Ok(new { total, logs });
        }

        [HttpGet("revenue/monthly")]
        public async Task<IActionResult> GetMonthlyRevenue()
        {
            var now = DateTime.UtcNow;
            var start = new DateTime(now.Year, now.Month, 1);
            var end = start.AddMonths(1);
            var logs = await _adminService.GetPaymentLogsByDateRangeAsync(start, end);
            var total = logs
    .Where(x => x.PaymentStatus == "Completed")
    .Sum(x => x.PaymentAmount ?? 0);
            return Ok(new { total, logs });
        }

        [HttpGet("routine-feedbacks")]
        public async Task<IActionResult> GetAllFeedbacks()
        {
            var feedbacks = await _routineFeedbackService.GetAllFeedbacksAsync();
            return Ok(feedbacks);
        }

    }
}
