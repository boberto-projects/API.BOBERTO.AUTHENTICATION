using api_authentication_boberto.Models.Config;
using RestEase;

namespace api_authentication_boberto.Integrations.ZenviaApiClient
{
    public static class ZenviaApiBuilder
    {
        public static void BuildZenviaAPI(this IServiceCollection services, IConfigurationRoot config) 
        {
            var configOptions = config.GetSection("ZenviaApiConfig").Get<ZenviaApiConfig>();

            IZenviaApi api = RestClient.For<IZenviaApi>(configOptions.Url);
            api.ApiKey = configOptions.ApiKey;

            services.AddSingleton(api);
        }
    }
}
