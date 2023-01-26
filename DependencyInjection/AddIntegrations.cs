using api_authentication_boberto.Integrations.DiscordApiClient;
using api_authentication_boberto.Integrations.SMSAdbTester;
using api_authentication_boberto.Integrations.ZenviaApiClient;
using api_authentication_boberto.Services.Discord;
using api_authentication_boberto.Services.Email;
using api_authentication_boberto.Services.Zenvio;

namespace api_authentication_boberto.DependencyInjection
{
    public static partial class DependencyInjection
    {
        public static void AddIntegrations(this WebApplicationBuilder builder)
        {
            var config = builder.Configuration;
            builder.Services.BuildZenviaAPI(config);
            builder.Services.BuildDiscordAPI(config);
            builder.Services.BuildADBTesterBuilder(config);

            builder.Services.AddSingleton<IEmailService, EmailService>();
            builder.Services.AddSingleton<IDiscordService, DiscordService>();
            builder.Services.AddSingleton<ZenvioService>();
        }
    }
}
