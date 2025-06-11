using System;
using System.Threading.Tasks;
using Contract.Helpers;
using Microsoft.Extensions.Options;
using Service.Interfaces;
using StackExchange.Redis;

namespace Service.Services
{
    public class RedisService : IRedisService
    {
        private readonly Lazy<ConnectionMultiplexer> _muxer;
        private IDatabase Db => _muxer.Value.GetDatabase();
        private IServer Server => _muxer.Value.GetServer(_muxer.Value.GetEndPoints().First());

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

        public async Task<List<string>> GetValuesByPatternAsync(string pattern)
        {
            var keys = Server.Keys(pattern: pattern).ToArray();
            var result = new List<string>();
            foreach (var key in keys)
            {
                var val = await Db.StringGetAsync(key);
                if (!val.IsNullOrEmpty) result.Add(val!);
            }
            return result;
        }
    }
}
