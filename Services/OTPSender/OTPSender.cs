namespace api_authentication_boberto.Services.OTPSender
{
    public class OTPSender : IEnviarCodigoDuploFator
    {
        private ZenvioService _zenvioService;
        private DiscordService _discordService;
        private IEmailService _emailService;
        private ApiConfig _apiConfig;
        private ISmsAdbTesterApi _smsAdbTesterApi;

        private ResourcesConfig _resourceConfig;
        public OTPSender(ZenvioService zenvioService,
            IEmailService emailService,
            DiscordService discordService,
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

        public void EnviarCodigoSMS(string numeroCelular, string codigo)
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
                 new Integrations.SMSAdbTester.Request.SendAdbTesterMessageRequest()
                 {
                     Message = $"ApiAuthBoberto: Seu código é {codigo}"
                 });

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
