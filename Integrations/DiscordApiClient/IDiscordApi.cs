using api_authentication_boberto.Integrations.DiscordApiClient.Request;
using RestEase;

namespace api_authentication_boberto.Integrations.DiscordApiClient
{
    public interface IDiscordApi
    {
        [Post("webhooks/{webHookId}/{webHookToken}")]
        Task EnviarMensagem([Path] string webHookId, [Path] string webHookToken, [Body] DiscordRequest request);
    }
}
