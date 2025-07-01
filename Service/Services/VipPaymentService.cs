using Microsoft.EntityFrameworkCore;
using Net.payOS;
using Net.payOS.Types;
using Newtonsoft.Json;
using Repository.Models;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            if (vipPackage == null)
                throw new Exception("Không tìm thấy gói VIP!");

            //dùng UnixTimeMilliseconds làm orderCode tránh dupe
            long orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var item = new ItemData(vipPackage.Name, 1, (int)vipPackage.Price);
            var items = new List<ItemData> { item };

            var baseUrl = "https://exe201skincarefenew.vercel.app";
            var paymentData = new PaymentData(
                orderCode,
                (int)vipPackage.Price,
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
                TransactionId = orderCode.ToString(),
                PaymentStatus = "Pending",
                PaymentAmount = vipPackage.Price,
                PaymentDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                VippackageId = vipPackage.Id
            };
            _db.PaymentLogs.Add(log);
            await _db.SaveChangesAsync();

            return result.checkoutUrl;
        }

        public async Task<bool> HandlePayOSWebhookAsync(WebhookType webhookBody)
        {

            WebhookData webhookData;
            try
            {

                Console.WriteLine("Received webhook: " + JsonConvert.SerializeObject(webhookBody));
                webhookData = _payOS.verifyPaymentWebhookData(webhookBody);
                Console.WriteLine("Verified webhook data: " + JsonConvert.SerializeObject(webhookData));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Verify webhook failed: {ex}");
                return false;
            }

            var transactionId = webhookData.orderCode.ToString();
            Console.WriteLine($"Looking for transactionId = {transactionId} in DB...");
            var log = await _db.PaymentLogs.FirstOrDefaultAsync(x => x.TransactionId == transactionId);
            if (log == null)
            {
                Console.WriteLine($"Transaction {transactionId} not found in DB.");
                return false;
            }

            bool isPaid = webhookBody.success
                          && webhookData.code == "00"
                          && webhookData.desc?.Trim().ToLower() == "success";

            Console.WriteLine($"Payment status: {(isPaid ? "Completed" : "Failed")}");
            log.PaymentStatus = isPaid ? "Completed" : "Failed";
            log.UpdatedAt = DateTime.UtcNow;
            _db.Entry(log).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to update PaymentLog: {ex}");
                return false;
            }

            if (isPaid)
            {
                try
                {
                    var userVip = await _db.UserVips.FirstOrDefaultAsync(x => x.UserId == log.UserId && x.VippackageId == log.VippackageId);

                    var now = DateTime.UtcNow;
                    if (userVip == null)
                    {
                        Console.WriteLine("Creating new UserVip record.");
                        _db.UserVips.Add(new UserVip
                        {
                            UserId = log.UserId.Value,
                            VippackageId = log.VippackageId.Value,
                            PurchaseDate = now,
                            ExpirationDate = now.AddMonths(1)
                        });
                    }
                    else
                    {
                        Console.WriteLine("Updating existing UserVip expiration.");
                        if (userVip.ExpirationDate != null && userVip.ExpirationDate > now)
                            userVip.ExpirationDate = userVip.ExpirationDate.Value.AddMonths(1);
                        else
                            userVip.ExpirationDate = now.AddMonths(1);

                        userVip.PurchaseDate = now;
                        _db.UserVips.Update(userVip);
                    }
                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to update UserVip: {ex}");
                    return false;
                }
            }

            return true;
        }
    }
}