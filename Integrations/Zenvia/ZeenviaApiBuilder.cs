using api_authentication_boberto.Models.Config;
using Microsoft.Extensions.Options;
using RestEase;
using System.Configuration;

namespace api_authentication_boberto.Integrations.Zenvia
{
    public static class ZeenviaApiBuilder
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
