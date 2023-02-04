namespace API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Config
{
    [Serializable]

    public class ApiConfig
    {
        public bool Swagger { get; set; }
        public Authorization AuthorizationConfig { get; set; }
        public ApiKeyAuthentication ApiKeyAuthenticationConfig { get; set; }
    }
    public class ApiKeyAuthentication
    {
        public string ApiHeader { get; set; }
        public bool Enabled { get; set; }
        public string CryptKey { get; set; }
    }
    public class Authorization
    {
        public bool Enabled { get; set; }
        public string ApiHeader { get; set; }
        public string ApiKey { get; set; }
    }
}

