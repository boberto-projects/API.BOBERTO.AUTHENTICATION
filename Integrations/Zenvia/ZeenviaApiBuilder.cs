using api_authentication_boberto.Models.Config;
using Microsoft.AspNetCore.Builder;
using RestEase;
using System.Security.Policy;

namespace api_authentication_boberto.Integrations.Zenvia
{
    public static class ZeenviaApiBuilder
    {
        public static void BuildZenviaAPI(this IServiceCollection services, IConfigurationRoot config) 
        {
            var configOptions = config.Get<ZenviaApiConfig>();

            IZenviaApi api = RestClient.For<IZenviaApi>(configOptions.Url);
            api.ApiKey = configOptions.ApiKey;

            services.AddSingleton<IZenviaApi>();
        }
    }
}
