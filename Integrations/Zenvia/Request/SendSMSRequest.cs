using Newtonsoft.Json;

namespace api_authentication_boberto.Integrations.Zenvia.Request
{
    public class SendSMSRequest
    {
        public class Content
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("text")]
            public string Text { get; set; }
        }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("contents")]
        public List<Content> Contents { get; set; }
    }
}
