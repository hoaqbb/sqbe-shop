using API.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace API.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _redis;
        private const int expiryDay = 7;

        public CacheService(IConnectionMultiplexer muxer)
        {
            _redis = muxer.GetDatabase();
        }

        public async Task<T> GetDataAsync<T>(string key)
        {
            var data = await _redis.StringGetAsync(key);
            if (!string.IsNullOrEmpty(data))
            {
                return JsonConvert.DeserializeObject<T>(data);
            }
            return default;
        }

        public async Task<T> SetDataAsync<T>(string key, T value)
        {
            var isSet = await _redis.StringSetAsync(key, JsonConvert.SerializeObject(value), TimeSpan.FromDays(expiryDay));
            if (isSet)
                return value;
            return default;
        }

        public async Task<bool> RemoveDataAsync(string key)
        {
            bool isKeyExist = await _redis.KeyExistsAsync(key);
            if (isKeyExist == true)
            {
                return await _redis.KeyDeleteAsync(key);
            }
            return false;
        }
    }
}
