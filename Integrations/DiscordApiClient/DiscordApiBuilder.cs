using api_authentication_boberto.Integrations.ZenviaApiClient;
using api_authentication_boberto.Models.Config;
using RestEase;

namespace api_authentication_boberto.Integrations.DiscordApiClient
{
    public static class DiscordApiBuilder
    {
        public static void BuildZenviaAPI(this IServiceCollection services, IConfigurationRoot config)
        {
            var configOptions = config.GetSection("DiscordAPIConfig").Get<DiscordAPIConfig>();

            IZenviaApi api = RestClient.For<IZenviaApi>(configOptions.Url);
            api.ApiKey = configOptions.ApiKey;

            services.AddSingleton(api);
        }
    }
}
