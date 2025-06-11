using System;
using System.Threading.Tasks;
using Contract.DTOs;
using Contract.Helpers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Service.Interfaces;
using StackExchange.Redis;

namespace Service.Services
{
    public class RedisService : IRedisService
    {
        private readonly Lazy<ConnectionMultiplexer> _muxer;
        private IDatabase Db => _muxer.Value.GetDatabase();

        public RedisService(IOptions<RedisSettings> options)
        {
            var conf = options.Value;
            //khởi tạo kết nối redis (lazy để chỉ lấy khi cần)
            _muxer = new Lazy<ConnectionMultiplexer>(() =>
                ConnectionMultiplexer.Connect(conf.ConnectionString));
        }

        //set 1 key mới có expiry và value (blacklist tự refresh)
        public async Task SetStringAsync(string key, string value, TimeSpan? expiry = null)
            => await Db.StringSetAsync(key, value, expiry);
        //đọc key
        public async Task<string> GetStringAsync(string key)
            => await Db.StringGetAsync(key);
        //reoke key
        public async Task DeleteKeyAsync(string key)
            => await Db.KeyDeleteAsync(key);

        //track user
        public async Task AddUserToLoginSetAsync(string setKey, string userId)
        {
            await Db.SetAddAsync(setKey, userId);
            //refresh moi lan add lai key de ko trung user/set
            await Db.KeyExpireAsync(setKey, TimeSpan.FromDays(2));
        }

        public async Task<long> GetLoginSetCountAsync(string setKey)
        {
            return await Db.SetLengthAsync(setKey);
        }

        public async Task SetKeyExpireAsync(string setKey, TimeSpan expiry)
        {
            await Db.KeyExpireAsync(setKey, expiry);
        }

        public async Task AddLoginHistoryAsync(Guid userId, string ip, string device, DateTime loginAt)
        {
            var key = $"loginhistory:{userId}";
            var history = new LoginHistoryDto
            {
                Ip = ip,
                Device = device,
                LoginAt = loginAt
            };
            var json = JsonConvert.SerializeObject(history);
            await Db.ListRightPushAsync(key, json); 
        }

        public async Task<List<LoginHistoryDto>> GetLoginHistoryAsync(Guid userId)
        {
            var key = $"loginhistory:{userId}";
            var items = await Db.ListRangeAsync(key, 0, -1); 
            return items
                .Select(i => JsonConvert.DeserializeObject<LoginHistoryDto>(i.ToString()))
                .Where(i => i != null)
                .ToList();
        }

    }
}
