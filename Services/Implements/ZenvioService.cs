using api_authentication_boberto.Exceptions;
using api_authentication_boberto.Integrations.ZenviaApiClient;
using api_authentication_boberto.Models;
using api_authentication_boberto.Models.Config;
using Microsoft.Extensions.Options;
using static api_authentication_boberto.Integrations.ZenviaApiClient.SendSMSRequest;

namespace api_authentication_boberto.Services.Implements
{
    public class ZenvioService 
    {
        private IOptions<ZenviaApiConfig> zenviaApiConfig;
        private GerenciadorZenvio gerenciadorZenvio;
        private IZenviaApi zenviaApiClient;

        public ZenvioService(IOptions<ZenviaApiConfig> zenviaApiConfig, GerenciadorZenvio gerenciadorZenvio, IZenviaApi zenviaApiClient)
        {
            this.zenviaApiConfig = zenviaApiConfig;
            this.gerenciadorZenvio = gerenciadorZenvio;
            this.zenviaApiClient = zenviaApiClient;
        }

        public async Task<SendSMSResponse> EnviarSMS(string numeroCelular, string texto)
        {
            if (zenviaApiConfig.Value.Enabled == false)
            {
                throw new CustomException(StatusCodeEnum.Interno, "Recurso envio de SMS desativado");
            }

            gerenciadorZenvio.IncrementarTentativa();

            if (gerenciadorZenvio.AtingiuLimiteMaximoDeTentativas())
            {
                throw new CustomException(StatusCodeEnum.Interno, "Limite máximo de SMS diário atingido.");
            }

            var conteudoMensagem = new List<Content>();

            conteudoMensagem.Add(new()
            {
                Type = "text",
                Text = texto
            });

            return await zenviaApiClient.EnviarSMS(new()
            {
                To = numeroCelular,
                From = zenviaApiConfig.Value.Alias,
                Contents = conteudoMensagem
            });
        }

        public async Task EnviarSMSCodigo(string numeroCelular, string codigo)
        {
            var conteudoMensagem = $"ApiAuthBoberto: Seu codigo e {codigo}";

            await EnviarSMS(numeroCelular, conteudoMensagem);
        }
    }
}
