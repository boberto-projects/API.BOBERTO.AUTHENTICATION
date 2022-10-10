using Newtonsoft.Json;

namespace api_authentication_boberto.Integrations.Zenvia.Response
{
    public class SendSMSResponse
    {
        public class Content
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("text")]
            public string Text { get; set; }

            [JsonProperty("payload")]
            public string Payload { get; set; }

            [JsonProperty("encodingStrategy")]
            public string EncodingStrategy { get; set; }
        }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("from")]
            public string From { get; set; }

            [JsonProperty("to")]
            public string To { get; set; }

            [JsonProperty("direction")]
            public string Direction { get; set; }

            [JsonProperty("channel")]
            public string Channel { get; set; }

            [JsonProperty("contents")]
            public List<Content> Contents { get; set; }

            [JsonProperty("timestamp")]
            public DateTime Timestamp { get; set; }
    }
}
