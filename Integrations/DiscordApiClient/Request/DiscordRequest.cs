using Newtonsoft.Json;

namespace api_authentication_boberto.Integrations.DiscordApiClient
{
    public class DiscordRequest
    {
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
