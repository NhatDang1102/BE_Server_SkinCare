using System;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IRedisService
    {
        Task SetStringAsync(string key, string value, TimeSpan? expiry = null);
        Task<string> GetStringAsync(string key);
        Task DeleteKeyAsync(string key);
    }
}
