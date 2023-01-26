namespace api_authentication_boberto.Models.Config
{
    public class AppSettingsConfig
    {
        public ConnectionStringsConfig ConnectionStrings { get; set; }
        public ApiConfig ApiConfig { get; set; }
        public ResourcesConfig ResourcesConfig { get; set; }
        public SmtpConfig SmtpConfig { get; set; }
        public JwtConfig JwtConfig { get; set; }
        public TwoFactorConfig TwoFactorConfig { get; set; }
        public UserSecurityConfig UserSecurityConfig { get; set; }
        public ZenvioSecurityConfig ZenvioSecurityConfig { get; set; }
        public DiscordAPIConfig DiscordApiConfig { get; set; }
        public ZenviaApiConfig ZenviaApiConfig { get; set; }
    }
}
