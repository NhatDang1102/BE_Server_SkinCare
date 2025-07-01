using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using Newtonsoft.Json;
using Service.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

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
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("id")
                ?? User.FindFirstValue("sub");

            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized("Không tìm thấy user");

            var url = await _vipPaymentService.CreateVipPaymentLinkAsync(userId);
            return Ok(new { checkoutUrl = url });
        }

        [HttpPost("receive-hook")]
        public async Task<IActionResult> PayOSWebhook([FromBody] WebhookType webhookBody)
        {
            Console.WriteLine("Webhook received: " + JsonConvert.SerializeObject(webhookBody));
            var handled = await _vipPaymentService.HandlePayOSWebhookAsync(webhookBody);
            Console.WriteLine("Webhook handled: " + handled);
            return Ok(new { success = handled });
        }

        [HttpPost("cancel-link")]
        [Authorize]
        public async Task<IActionResult> CancelLink([FromBody] CancelLinkDto dto)
        {
            var success = await _vipPaymentService.CancelVipPaymentAsync(dto.TransactionId, dto.CancellationReason);
            return Ok(new { success });
        }

        public class CancelLinkDto
        {
            public string TransactionId { get; set; }
            public string? CancellationReason { get; set; }
        }


    }
}