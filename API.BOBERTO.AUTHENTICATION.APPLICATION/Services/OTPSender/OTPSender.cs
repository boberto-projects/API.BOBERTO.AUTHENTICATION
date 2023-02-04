using Microsoft.Extensions.Options;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.Services.OTPSender
{
    /// <summary>
    /// We will delegate all services integrations to API.BOBERTO.SERVICES
    /// </summary>
    public class OTPSender : IOTPSender
    {
        //public OTPSender(ZenvioService zenvioService,
        //    IEmailService emailService,
        //    IDiscordService discordService,
        //    IOptions<ResourcesConfig> resourceConfig,
        //    IOptions<ApiConfig> apiConfig,
        //    ISmsAdbTesterApi smsAdbTesterApi
        //    )
        //{
        //    _emailService = emailService;
        //    _zenvioService = zenvioService;
        //    _discordService = discordService;
        //    _apiConfig = apiConfig.Value;
        //    _resourceConfig = resourceConfig.Value;
        //    _smsAdbTesterApi = smsAdbTesterApi;
        //}

        //public void SendSMS(string phoneNumber, string code)
        //{
        //    ///Como não podemos ultrapassar a cota de sms mensal e não temos opção de setar isso no zenvio, 
        //    ///vamos substituir o sms pela a api do discord.
        //    var resources = _resourceConfig.Resources.ToDictionary(x => x.Key);
        //    bool alternativaAoSMS = resources["PreferirAlternativaAoSMS"].Enabled;
        //    if (alternativaAoSMS)
        //    {
        //        _smsAdbTesterApi.SendSMS(
        //         new SendAdbTesterMessageRequest()
        //         {
        //             Message = $"ApiAuthBoberto: Your code is {code}"
        //         });
        //        return;
        //    }
        //    _zenvioService.SendSMSCode(phoneNumber, code);
        //}

        //public void SendEmail(string email, string code)
        //{
        //    var to = email;
        //    var subject = "[TESTE] ApiAuthBoberto";
        //    var html = $"<h1> ApiAuthBoberto: Your code is {code}</h1>";
        //    _emailService.Send(to, subject, html);
        //}
        public void SendEmail(string email, string code)
        {
            throw new NotImplementedException();
        }

        public void SendSMS(string phoneNumber, string code)
        {
            throw new NotImplementedException();
        }
    }
}
