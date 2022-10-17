using api_authentication_boberto.Implements;
using api_authentication_boberto.Interfaces;
using api_authentication_boberto.Models.Cache;
using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace api_authentication_boberto.Services.Implements
{
    public class GerenciadorZenvio
    {
        private IRedisService redisService { get; set; }
        private GerenciadorZenvioConfig zenvioConfig { get; set; }

        private const string CACHE_ZENVIO = "COUNT_SMS_GLOBAL_SENDED";

        public GerenciadorZenvio(IRedisService _redisService, IOptions<GerenciadorZenvioConfig> _zenvioConfig)
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
