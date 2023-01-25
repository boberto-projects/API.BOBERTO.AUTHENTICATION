namespace api_authentication_boberto.Models
{
    public class GetApiKeyModel
    {
        public string ApiKey { get; set; }
        public string ApiKeyHashed { get; set; }
        public string[] Scopes { get; set; }
    }
}
