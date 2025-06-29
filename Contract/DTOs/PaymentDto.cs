using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.DTOs
{
    public record CreatePaymentLinkRequest(
         string productName,
         string description,
         int price,
         string returnUrl,
         string cancelUrl
    );

    public record Response(
        int error,
        string message,
        object? data
    );

    public record ConfirmWebhook(
        string webhook_url
    );
    public class PayOSWebhook
    {
        public string code { get; set; }
        public string desc { get; set; }
        public bool success { get; set; }
        public object data { get; set; }
        public string signature { get; set; }
    }

    public class PayOSWebhookData
    {
        public int orderCode { get; set; }
        public int amount { get; set; }
        public string description { get; set; }
        public string paymentLinkId { get; set; }
        // Thêm các field khác nếu muốn dùng
    }
}
