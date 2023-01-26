using api_authentication_boberto.Integrations.SMSAdbTester;
using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Services.Discord;
using api_authentication_boberto.Services.Email;
using api_authentication_boberto.Services.SenderService;
using api_authentication_boberto.Services.Zenvio;
using Microsoft.Extensions.Options;

namespace api_authentication_boberto.Services.OTPSender
{
    public class OTPSender : IOTPSender
    {
        private ZenvioService _zenvioService;
        private IDiscordService _discordService;
        private IEmailService _emailService;
        private ApiConfig _apiConfig;
        private ISmsAdbTesterApi _smsAdbTesterApi;

        private ResourcesConfig _resourceConfig;
        public OTPSender(ZenvioService zenvioService,
            IEmailService emailService,
            IDiscordService discordService,
            IOptions<ResourcesConfig> resourceConfig,
            IOptions<ApiConfig> apiConfig,
            ISmsAdbTesterApi smsAdbTesterApi
            )
        {
            _emailService = emailService;
            _zenvioService = zenvioService;
            _discordService = discordService;
            _apiConfig = apiConfig.Value;
            _resourceConfig = resourceConfig.Value;
            _smsAdbTesterApi = smsAdbTesterApi;
        }

        public void SendSMS(string numeroCelular, string codigo)
        {
            ///Como não podemos ultrapassar a cota de sms mensal e não temos opção de setar isso no zenvio, 
            ///vamos substituir o sms pela a api do discord.
            var resources = _resourceConfig.Resources;
            bool alternativaAoSMS = false;
            foreach (var resource in resources)
            {
                if (resource.Key.Equals("PreferirAlternativaAoSMS"))
                {
                    alternativaAoSMS = resource.Enabled;
                    // _discordService.EnviarCodigo(codigo);
                    break;
                }
            }

            if (alternativaAoSMS)
            {
                _smsAdbTesterApi.EnviarSMS(
                 new SendAdbTesterMessageRequest()
                 {
                     Message = $"ApiAuthBoberto: Seu código é {codigo}"
                 });

                return;
            }
            _zenvioService.SendSMSCode(numeroCelular, codigo);
        }

        public void SendEmail(string email, string codigo)
        {
            var to = email;
            var subject = "[TESTE] ApiAuthBoberto";
            var html = $"<h1> ApiAuthBoberto: Seu código é {codigo}</h1>";
            _emailService.Send(to, subject, html);
        }
    }
}
