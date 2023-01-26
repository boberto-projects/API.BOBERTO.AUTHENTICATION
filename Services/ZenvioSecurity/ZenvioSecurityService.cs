using api_authentication_boberto.Models.Cache;
using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Services.Redis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace api_authentication_boberto.Services.ZenvioSecurity
{
    public class ZenvioSecurityService : IZenvioSecurityService
    {
        private IRedisService redisService { get; set; }
        private ZenvioSecurityConfig zenvioConfig { get; set; }

        private const string CACHE_ZENVIO = "COUNT_SMS_GLOBAL_SENDED";

        public ZenvioSecurityService(IRedisService _redisService, IOptions<ZenvioSecurityConfig> _zenvioConfig)
        {
            redisService = _redisService;
            zenvioConfig = _zenvioConfig.Value;
            CreateCache();
        }

        public bool ReachedMaximumLimitOfAttempts()
        {
            var obterCacheZenvio = GetCahe();
            var tentativasDeEnvio = obterCacheZenvio.Tentativas;
            if (tentativasDeEnvio >= zenvioConfig.MaximumAttempts)
            {
                return true;
            }
            return false;
        }

        public void IncrementAttemp()
        {
            if (ReachedMaximumLimitOfAttempts())
            {
                return;
            }
            var oberCacheZenvio = GetCahe();
            oberCacheZenvio.Tentativas += 1;
            oberCacheZenvio.UltimaTentativa = DateTime.Now;
            redisService.Set(CACHE_ZENVIO, oberCacheZenvio);
        }

        public TimeSpan GetBlockTime()
        {
            return TimeSpan.FromSeconds(zenvioConfig.SecondsExpiration);
        }

        public TimeSpan GetBlockTimeRemaining()
        {
            var dataAtual = DateTime.Now;
            var dataFinal = dataAtual.AddSeconds(zenvioConfig.SecondsExpiration);
            return dataFinal.Subtract(dataAtual);
        }
        public ZenvioCacheModel GetCahe()
        {
            return redisService.Get<ZenvioCacheModel>(CACHE_ZENVIO);
        }
        public void CreateCache()
        {
            var cacheZenvio = new ZenvioCacheModel();
            cacheZenvio.UltimaTentativa = DateTime.MinValue;
            var cacheOptions = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(zenvioConfig.SecondsExpiration)
            };
            if (redisService.Exists(CACHE_ZENVIO) == false)
            {
                redisService.Set(CACHE_ZENVIO, cacheZenvio, cacheOptions);
            }
        }
    }
}
