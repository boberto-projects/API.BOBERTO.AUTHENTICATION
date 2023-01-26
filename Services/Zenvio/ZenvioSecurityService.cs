using api_authentication_boberto.Models.Cache;
using api_authentication_boberto.Services.RedisService;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace api_authentication_boberto.Services.Zenvio
{
    public class ZenvioSecurityService
    {
        private IRedisService redisService { get; set; }
        private ZenvioSecurityServiceConfig zenvioConfig { get; set; }

        private const string CACHE_ZENVIO = "COUNT_SMS_GLOBAL_SENDED";

        public ZenvioSecurityService(IRedisService _redisService, IOptions<ZenvioSecurityServiceConfig> _zenvioConfig)
        {
            redisService = _redisService;
            zenvioConfig = _zenvioConfig.Value;
            CriarCacheZenvio();
        }

        public bool AtingiuLimiteMaximoDeTentativas()
        {
            var obterCacheZenvio = ObterCacheZenvio();
            var tentativasDeEnvio = obterCacheZenvio.Tentativas;
            if (tentativasDeEnvio >= zenvioConfig.QuantidadeMaximaTentativas)
            {
                return true;
            }
            return false;
        }

        public void IncrementarTentativa()
        {
            if (AtingiuLimiteMaximoDeTentativas())
            {
                return;
            }
            var oberCacheZenvio = ObterCacheZenvio();
            oberCacheZenvio.Tentativas += 1;
            oberCacheZenvio.UltimaTentativa = DateTime.Now;
            redisService.Set(CACHE_ZENVIO, oberCacheZenvio);
        }

        public void LimparTentativas(string chave)
        {
            redisService.Clear(chave);
        }

        public TimeSpan ObterTempoBloqueio()
        {
            return TimeSpan.FromSeconds(zenvioConfig.SegundosExpiracao);
        }

        public TimeSpan ObterTempoBloqueioRestante()
        {
            var dataAtual = DateTime.Now;
            var dataFinal = dataAtual.AddSeconds(zenvioConfig.SegundosExpiracao);
            return dataFinal.Subtract(dataAtual);
        }
        public ZenvioCacheModel ObterCacheZenvio()
        {
            return redisService.Get<ZenvioCacheModel>(CACHE_ZENVIO);
        }
        private void CriarCacheZenvio()
        {
            var cacheZenvio = new ZenvioCacheModel();
            cacheZenvio.UltimaTentativa = DateTime.MinValue;
            var cacheOptions = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(zenvioConfig.SegundosExpiracao)
            };
            if (redisService.Exists(CACHE_ZENVIO) == false)
            {
                redisService.Set(CACHE_ZENVIO, cacheZenvio, cacheOptions);
            }
        }
    }
}
