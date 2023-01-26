using Newtonsoft.Json;

namespace api_authentication_boberto.Integrations.SMSAdbTester
{
    public class SendAdbTesterMessageRequest
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
