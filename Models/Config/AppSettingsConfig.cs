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
        public GerenciadorAutenticacaoConfig GerenciadorAutenticacaoConfig { get; set; }
        public GerenciadorZenvioConfig GerenciadorZenvioConfig { get; set; }
        public DiscordAPIConfig DiscordApiConfig { get; set; }
        public ZenviaApiConfig ZenviaApiConfig { get; set; }
        public string AllowedHosts { get; set; }
    }
}
