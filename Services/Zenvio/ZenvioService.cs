

using api_authentication_boberto.Exceptions;
using api_authentication_boberto.Integrations.ZenviaApiClient;
using api_authentication_boberto.Integrations.ZenviaApiClient.Request;
using api_authentication_boberto.Integrations.ZenviaApiClient.Response;
using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Models.Enums;
using api_authentication_boberto.Services.ZenvioSecurity;
using Microsoft.Extensions.Options;

namespace api_authentication_boberto.Services.Zenvio
{
    public class ZenvioService
    {
        private IOptions<ZenviaApiConfig> zenviaApiConfig;
        private IZenvioSecurityService gerenciadorZenvio;
        private IZenviaApi zenviaApiClient;

        public ZenvioService(IOptions<ZenviaApiConfig> zenviaApiConfig, IZenvioSecurityService gerenciadorZenvio, IZenviaApi zenviaApiClient)
        {
            this.zenviaApiConfig = zenviaApiConfig;
            this.gerenciadorZenvio = gerenciadorZenvio;
            this.zenviaApiClient = zenviaApiClient;
        }

        public async Task<SendSMSResponse> SendSMS(string phoneNumber, string texto)
        {
            if (zenviaApiConfig.Value.Enabled == false)
            {
                throw new CustomException(StatusCodeEnum.INTERN, "Recurso envio de SMS desativado");
            }

            gerenciadorZenvio.IncrementAttemp();

            if (gerenciadorZenvio.ReachedMaximumLimitOfAttempts())
            {
                throw new CustomException(StatusCodeEnum.INTERN, "Limite máximo de SMS diário atingido.");
            }

            var messageContent = new List<SendSMSRequest.Content>() { new()
            {
                Type = "text",
                Text = texto
            }};

            return await zenviaApiClient.SendSMS(new()
            {
                To = phoneNumber,
                From = zenviaApiConfig.Value.Alias,
                Contents = messageContent
            });
        }

        public async Task SendSMSCode(string phoneNumber, string code)
        {
            var messageContent = $"ApiAuthBoberto: Your code is {code}";

            await SendSMS(phoneNumber, messageContent);
        }
    }
}
