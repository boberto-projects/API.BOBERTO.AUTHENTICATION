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
        /// <summary>
        /// TODO: Wrong way to do this. Api boberto services already has discord integration. We need to remove discord service.
        /// </summary>
        public async Task SendMessage(string text)
        {
            if (discordApiConfig.Value.Enabled == false)
            {
                throw new CustomException(StatusCodeEnum.INTERN, "Discord web hook disabled.");
            }

            var discordWebHookID = discordApiConfig.Value.WebHookId;
            var discordWebHookToken = discordApiConfig.Value.WebHookToken;
            var discordRequest = new DiscordRequest()
            {
                Content = text
            };
            await discordApi.EnviarMensagem(discordWebHookID, discordWebHookToken, discordRequest);
        }

        public async Task SendCode(string code)
        {
            var conteudoMensagem = $"ApiAuthBoberto: Your code is {code}";
            await SendMessage(conteudoMensagem);
        }
    }
}