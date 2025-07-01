using System;
using System.Threading.Tasks;
using Contract.DTOs;
using Net.payOS.Types;

namespace Service.Interfaces
{
    public interface IVipPaymentService
    {
        Task<string> CreateVipPaymentLinkAsync(Guid userId);
        Task<bool> HandlePayOSWebhookAsync(WebhookType webhookBody);
            }
}
