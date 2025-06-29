using System;
using System.Threading.Tasks;
using Contract.DTOs;

namespace Service.Interfaces
{
    public interface IVipPaymentService
    {
        Task<string> CreateVipPaymentLinkAsync(Guid userId);
        Task<bool> HandlePayOSWebhookAsync(PayOSWebhook data); 
    }
}
