using api_authentication_boberto.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Drawing;
using System.Text.Json;
using System.Threading;

namespace api_authentication_boberto.Implements
{
    public class RedisService : IRedisService
    {
        private readonly IDistributedCache _redisCache;

        public RedisService(IDistributedCache redisDatabase)
        {
            _redisCache = redisDatabase;
        }

        public T Get<T>(string chave)
        {
            var value = _redisCache.GetString(chave);

            if (value != null)
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            return default;

        }

        public T Set<T>(string chave, T valor, TimeSpan expiracao)
        {
            var timeOut = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
                SlidingExpiration = expiracao
            };

            _redisCache.SetString(chave, JsonSerializer.Serialize(valor), timeOut);
            return valor;
        }

        public T Set<T>(string chave, T valor)
        {
            _redisCache.SetString(chave, JsonSerializer.Serialize(valor));
            return valor;
        }

        public bool Clear(string chave)
        {
            _redisCache.Remove(chave);
            return default;
        }

        public bool Exists(string chave)
        {
            return _redisCache.Get(chave) != null;
        }

        public T Set<T>(string chave, T valor, DistributedCacheEntryOptions cacheOptions)
        {
            _redisCache.SetString(chave, JsonSerializer.Serialize(valor), cacheOptions);
            return valor;
        }
    }
}
