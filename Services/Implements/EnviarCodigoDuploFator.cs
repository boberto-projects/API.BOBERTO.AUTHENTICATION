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
        private IEmailService _emailService;
        private ApiConfig _apiConfig;

        public EnviarCodigoDuploFator(ZenvioService zenvioService, 
            IEmailService emailService,
            DiscordService discordService, IOptions<ApiConfig> apiConfig)
        {
            _emailService = emailService;
            _zenvioService = zenvioService;
            _discordService = discordService;
            _apiConfig = apiConfig.Value;
        }

        public void EnviarCodigoSMS(string numeroCelular, string codigo)
        {
            ///Como não podemos ultrapassar a cota de sms mensal e não temos opção de setar isso no zenvio, 
            ///vamos substituir o sms pela a api do discord.
          
            if (_apiConfig.PreferirDiscordAoSMS)
            {
                _discordService.EnviarCodigo(codigo);
                return;
            }
            _zenvioService.EnviarSMSCodigo(numeroCelular, codigo);
        }
        
        public void EnviarCodigoEmail(string email, string codigo)
        {
            var to = email;
            var subject = "[TESTE] ApiAuthBoberto";
            var html = $"<h1> ApiAuthBoberto: Seu código é {codigo}</h1>";
            _emailService.Send(to, subject, html);
        }
    }
}
