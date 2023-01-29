namespace API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Config
{
    [Serializable]

    public class ApiConfig
    {
        public bool Swagger { get; set; }
        public Authorization Authorization { get; set; }
        public ApiKeyAuthentication ApiKeyAuthentication { get; set; }
    }
    public class ApiKeyAuthentication
    {
        public bool Enabled { get; set; }
        public string Key { get; set; }

    }
    public class Authorization
    {
        public bool Activate { get; set; }
        public string ApiHeader { get; set; }
        public string ApiKey { get; set; }
    }
}

