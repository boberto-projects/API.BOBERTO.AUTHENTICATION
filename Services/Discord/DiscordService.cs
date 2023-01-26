using api_authentication_boberto.Exceptions;
using api_authentication_boberto.Integrations.DiscordApiClient;
using api_authentication_boberto.Integrations.DiscordApiClient.Request;
using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Models.Enums;
using Microsoft.Extensions.Options;

namespace api_authentication_boberto.Services.Discord
{
    public class DiscordService : IDiscordService
    {
        private IOptions<DiscordAPIConfig> discordApiConfig;
        private IDiscordApi discordApi;

        public DiscordService(IOptions<DiscordAPIConfig> discordApiConfig, IDiscordApi discordApi)
        {
            this.discordApiConfig = discordApiConfig;
            this.discordApi = discordApi;
        }

        public async Task SendMessage(string texto)
        {
            if (discordApiConfig.Value.Enabled == false)
            {
                throw new CustomException(StatusCodeEnum.INTERN, "Recurso web hook discord desativado");
            }

            var discordWebHookID = discordApiConfig.Value.WebHookId;
            var discordWebHookToken = discordApiConfig.Value.WebHookToken;
            var discordRequest = new DiscordRequest()
            {
                Content = texto
            };
            await discordApi.EnviarMensagem(discordWebHookID, discordWebHookToken, discordRequest);
        }

        public async Task SendCode(string codigo)
        {
            var conteudoMensagem = $"ApiAuthBoberto: Seu codigo e {codigo}";
            await SendMessage(conteudoMensagem);
        }
    }
}