using System;
using System.Threading.Tasks;
using Contract.DTOs;

namespace Service.Interfaces
{
    public interface IRedisService
    {
        Task SetStringAsync(string key, string value, TimeSpan? expiry = null);
        Task<string> GetStringAsync(string key);
        Task DeleteKeyAsync(string key);

        //track user
        Task AddUserToLoginSetAsync(string setKey, string userId);
        Task<long> GetLoginSetCountAsync(string setKey);
        Task SetKeyExpireAsync(string setKey, TimeSpan expiry);
        Task AddLoginHistoryAsync(Guid userId, string ip, string device, DateTime loginAt);
        Task<List<LoginHistoryDto>> GetLoginHistoryAsync(Guid userId);
    }
}
