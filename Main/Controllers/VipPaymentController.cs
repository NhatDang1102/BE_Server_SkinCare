using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Contract.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace MainApp.Controllers
{
    [ApiController]
    [Route("SkinCare/vippayment")]
    public class VipPaymentController : ControllerBase
    {
        private readonly IVipPaymentService _vipPaymentService;

        public VipPaymentController(IVipPaymentService vipPaymentService)
        {
            _vipPaymentService = vipPaymentService;
        }

        [HttpPost("create-link")]
        [Authorize]
        public async Task<IActionResult> CreateLink()
        {
            //lay userid
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("id")
                ?? User.FindFirstValue("sub");
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized("Không tim thay user");

            var url = await _vipPaymentService.CreateVipPaymentLinkAsync(userId);
            return Ok(new { checkoutUrl = url });
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> PayOSWebhook([FromBody] PayOSWebhook data)
        {
            await _vipPaymentService.HandlePayOSWebhookAsync(data);
            return Ok(new { success = true });
        }
    }
}
