using api_authentication_boberto.Interfaces;
using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace api_authentication_boberto.Services.Implements
{
    public class GerenciadorAutenticacao : IGerenciadorAcesso
    {   
        private IRedisService redisService { get; set; }
        private IOptions<GerenciadorAutenticacaoConfig> autenticacaoConfig { get; set; }

        public GerenciadorAutenticacao(IRedisService redisService, IOptions<GerenciadorAutenticacaoConfig> gerenciadorAutenticacaoConfig)
        {
            this.redisService = redisService;
            autenticacaoConfig = gerenciadorAutenticacaoConfig;
        }
        public bool AtingiuLimiteMaximoDeTentativas(string chave)
        {
            var tentativasDeLogin = ObterTentativas(chave);
            if (tentativasDeLogin >= autenticacaoConfig.Value.QuantidadeMaximaTentativas)
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
            redisService.Set(chave, ultimaTentativa + 1);
        }

        public void LimparTentativas(string chave)
        {
            redisService.Clear(chave);
        }

        public TimeSpan ObterTempoBloqueio()
        {
            return TimeSpan.FromSeconds(autenticacaoConfig.Value.SegundosExpiracao);
        }

        public TimeSpan ObterTempoBloqueioRestante()
        {
            var dataAtual = DateTime.Now;
            var dataFinal = dataAtual.AddSeconds(autenticacaoConfig.Value.SegundosExpiracao);

            return dataAtual.Subtract(dataFinal);
        }

        public int ObterTentativas(string chave)
        {
            return redisService.Get<int>(chave);
        }
    }
}
