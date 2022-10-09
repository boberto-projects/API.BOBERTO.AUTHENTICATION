using api_authentication_boberto.Models.Integrations;
using RestEase;

namespace api_authentication_boberto.Integrations
{
    public interface IDiscordApi
    {

        [Post("webhooks/{webHookId}/{webHookToken}")]
        Task EnviarMensagem([Path] string webHookId, [Path] string webHookToken, [Body] DiscordRequest request);

    }
}
