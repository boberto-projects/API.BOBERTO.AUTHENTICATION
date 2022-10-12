using api_authentication_boberto.Integrations.Discord;
using api_authentication_boberto.Models.Config;
using RestEase;

namespace api_authentication_boberto.Integrations.DiscordApiClient
{
    public static class DiscordApiBuilder
    {
        public static void BuildZenviaAPI(this IServiceCollection services, IConfigurationRoot config)
        {
            var configOptions = config.GetSection("DiscordAPIConfig").Get<DiscordAPIConfig>();

            IDiscordApi api = RestClient.For<IDiscordApi>(configOptions.Url);

            services.AddSingleton(api);
        }
    }
}
