using Newtonsoft.Json;

namespace api_authentication_boberto.Integrations.DiscordApiClient.Request
{
    public class DiscordRequest
    {
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
