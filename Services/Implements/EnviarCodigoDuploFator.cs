using api_authentication_boberto.Interfaces;
using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using RestEase.Implementation;
using System.Resources;

namespace api_authentication_boberto.Services.Implements
{
    public class EnviarCodigoDuploFator : IEnviarCodigoDuploFator
    {
        private ZenvioService _zenvioService;
        private DiscordService _discordService;
        private IEmailService _emailService;
        private ApiConfig _apiConfig;

        private ResourcesConfig _resourceConfig;
        public EnviarCodigoDuploFator(ZenvioService zenvioService, 
            IEmailService emailService,
            DiscordService discordService,
            IOptions<ResourcesConfig> resourceConfig,
            IOptions<ApiConfig> apiConfig)
        {
            _emailService = emailService;
            _zenvioService = zenvioService;
            _discordService = discordService;
            _apiConfig = apiConfig.Value;
            _resourceConfig = resourceConfig.Value;
        }

        public void EnviarCodigoSMS(string numeroCelular, string codigo)
        {
            ///Como não podemos ultrapassar a cota de sms mensal e não temos opção de setar isso no zenvio, 
            ///vamos substituir o sms pela a api do discord.
            var resources = _resourceConfig.Resources;
            foreach (var resource in resources)
            {
                if (resource.Key.Equals("PreferirDiscordAoSMS") && resource.Value)
                {
                    _discordService.EnviarCodigo(codigo);
                    break;
                }
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
