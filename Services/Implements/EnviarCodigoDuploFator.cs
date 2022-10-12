using api_authentication_boberto.Interfaces;
using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace api_authentication_boberto.Services.Implements
{
    public class EnviarCodigoDuploFator : IEnviarCodigoDuploFator
    {
        private ZenvioService _zenvioService;
        private DiscordService _discordService;
        private IOptions<ApiConfig> _apiConfig;

        public EnviarCodigoDuploFator(ZenvioService zenvioService, DiscordService discordService, IOptions<ApiConfig> apiConfig)
        {
            _zenvioService = zenvioService;
            _discordService = discordService;
            _apiConfig = apiConfig;
        }

        public void EnviarCodigo(IUsuarioService usuario, string codigo)
        {
            var usuarioAutenticacoes = usuario.ObterAutenticacaoDuplaAtiva();

            var usarNumeroCelular = usuarioAutenticacoes.UsarNumeroCelular && usuarioAutenticacoes.NumeroCelular != null;
            var usarEmail = usuarioAutenticacoes.UsarEmail;
            ///Como não podemos ultrapassar a cota de sms mensal e não temos opção de setar isso no zenvio, 
            ///vamos substituir o sms pela a api do discord.
            if (usarNumeroCelular)
            {
                if (_apiConfig.Value.PreferirDiscordAoSMS)
                {
                    _discordService.EnviarCodigo(codigo);
                    return;
                }
                var numeroCelular = usuarioAutenticacoes.NumeroCelular;
                _zenvioService.EnviarSMSCodigo(numeroCelular, codigo);
            }
            ///sem email por agora.
            if (usarEmail)
            {

            }        
        }
    }
}
