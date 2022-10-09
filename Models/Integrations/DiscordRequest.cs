using Newtonsoft.Json;

namespace api_authentication_boberto.Models.Integrations
{
    public class DiscordRequest
    {
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
