using api_authentication_boberto.Integrations.DiscordApiClient;
using RestEase;

namespace api_authentication_boberto.Integrations.Discord
{
    public interface IDiscordApi
    {
        [Post("webhooks/{webHookId}/{webHookToken}")]
        Task EnviarMensagem([Path] string webHookId, [Path] string webHookToken, [Body] DiscordRequest request);
    }
}
