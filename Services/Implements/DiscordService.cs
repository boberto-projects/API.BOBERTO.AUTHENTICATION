using api_authentication_boberto.Exceptions;
using api_authentication_boberto.Integrations.Discord;
using api_authentication_boberto.Integrations.DiscordApiClient;
using api_authentication_boberto.Integrations.ZenviaApiClient;
using api_authentication_boberto.Models;
using api_authentication_boberto.Models.Config;
using Microsoft.Extensions.Options;

namespace api_authentication_boberto.Services.Implements
{
    public class DiscordService
    {
        private IOptions<DiscordAPIConfig> discordApiConfig;
        private IDiscordApi discordApi;

        public DiscordService(IOptions<DiscordAPIConfig> discordApiConfig, IDiscordApi discordApi)
        {
            this.discordApiConfig = discordApiConfig;
            this.discordApi = discordApi;
        }

        public async Task EnviarMensagem(string texto)
        {
            if (discordApiConfig.Value.Enabled == false)
            {
                throw new CustomException(StatusCodeEnum.Interno, "Recurso web hook discord desativado");
            }

            var discordWebHookID = discordApiConfig.Value.WebHookId;
            var discordWebHookToken = discordApiConfig.Value.WebHookToken;
            var discordRequest = new DiscordRequest()
            {
                Content = texto
            };
           await discordApi.EnviarMensagem(discordWebHookID, discordWebHookToken, discordRequest);
        }

        public async Task EnviarCodigo(string codigo)
        {
            var conteudoMensagem = $"ApiAuthBoberto: Seu codigo e {codigo}";
            await EnviarMensagem(conteudoMensagem);
        }
    }
}