using System;
using System.Linq;
using System.Threading.Tasks;
using Contract.DTOs;
using Microsoft.EntityFrameworkCore;
using Net.payOS;
using Net.payOS.Types;
using Repository.Models;
using Service.Interfaces;
using Newtonsoft.Json;

namespace Service.Services
{
    public class VipPaymentService : IVipPaymentService
    {
        private readonly SkinCareAppContext _db;
        private readonly PayOS _payOS;

        public VipPaymentService(SkinCareAppContext db, PayOS payOS)
        {
            _db = db;
            _payOS = payOS;
        }

        public async Task<string> CreateVipPaymentLinkAsync(Guid userId)
        {
            var vipPackage = await _db.VipPackages.OrderBy(x => x.Price).FirstOrDefaultAsync();
            if (vipPackage == null) throw new Exception("Không tìm thấy gói VIP!");

            var orderCode = $"{userId:N}{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
            int orderCodeInt = Math.Abs(orderCode.GetHashCode());

            var item = new ItemData(vipPackage.Name, 1, 2000);
            var items = new List<ItemData> { item };

            var baseUrl = "https://exe201-skincare-fe-new.onrender.com";  //deploy url

            var paymentData = new PaymentData(
                orderCodeInt,
                2000, //gia test
                "Thanh toán gói VIP EXE201",
                items,
           $"{baseUrl}/payment-page/cancel",
           $"{baseUrl}/payment-page/success"
            );

            var result = await _payOS.createPaymentLink(paymentData);

            var log = new PaymentLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TransactionId = orderCodeInt.ToString(),
                PaymentStatus = "Pending",
                PaymentAmount = 2000,
                PaymentDate = DateTime.Now,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                VippackageId = vipPackage.Id
            };
            _db.PaymentLogs.Add(log);
            await _db.SaveChangesAsync();

            return result.checkoutUrl;
        }

        public async Task HandlePayOSWebhookAsync(PayOSWebhook data)
        {
            
            var dataJson = data.data.ToString();
            var webhookData = JsonConvert.DeserializeObject<PayOSWebhookData>(dataJson);

            var log = await _db.PaymentLogs
                .FirstOrDefaultAsync(x => x.TransactionId == webhookData.orderCode.ToString());
            if (log == null) return;

            log.PaymentStatus = data.success ? "Completed" : "Failed";
            Console.WriteLine($"Webhook: Đang update log {log.TransactionId} thành {log.PaymentStatus}");

            log.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            if (data.success)
            {
                var userVip = await _db.UserVips
                    .FirstOrDefaultAsync(x => x.UserId == log.UserId && x.VippackageId == log.VippackageId);

                if (userVip == null)
                {
                    _db.UserVips.Add(new UserVip
                    {
                        UserId = log.UserId.Value,
                        VippackageId = log.VippackageId.Value,
                        PurchaseDate = DateTime.Now,
                        ExpirationDate = DateTime.Now.AddMonths(1)
                    });
                }
                else
                {
                    var now = DateTime.Now;
                    if (userVip.ExpirationDate != null && userVip.ExpirationDate > now)
                        userVip.ExpirationDate = userVip.ExpirationDate.Value.AddMonths(1);
                    else
                        userVip.ExpirationDate = now.AddMonths(1);

                    userVip.PurchaseDate = now; // cập nhật lại purchase date (nếu muốn)
                    _db.UserVips.Update(userVip);
                }

                await _db.SaveChangesAsync();
            }

        }
    }
}
