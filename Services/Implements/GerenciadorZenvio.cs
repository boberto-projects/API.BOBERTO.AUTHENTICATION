using api_authentication_boberto.Implements;
using api_authentication_boberto.Interfaces;
using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace api_authentication_boberto.Services.Implements
{
    public class GerenciadorZenvio : IGerenciadorAcesso
    {
        private IRedisService redisService { get; set; }
        private GerenciadorZenvioConfig zenvioConfig { get; set; }

        public GerenciadorZenvio(IRedisService _redisService, IOptions<GerenciadorZenvioConfig> _zenvioConfig)
        {
            redisService = _redisService;
            zenvioConfig = _zenvioConfig.Value;
        }

        public bool AtingiuLimiteMaximoDeTentativas(string chave)
        {
            var tentativasEnvioSMS = ObterTentativas(chave);
            if (tentativasEnvioSMS >= zenvioConfig.QuantidadeMaximaTentativas)
            {
                return true;
            }
            return false;
        }

        public void IncrementarTentativa(string chave)
        {
            if (AtingiuLimiteMaximoDeTentativas(chave))
            {
                return;
            }
            var ultimaTentativa = ObterTentativas(chave);

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(zenvioConfig.SegundosExpiracao),
            };

            redisService.Set(chave, ultimaTentativa + 1, cacheOptions);
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

        public int ObterTentativas(string chave)
        {
            return redisService.Get<int>(chave);
        }
    }
}
