using api_authentication_boberto.Models.Config;
using RestEase;

namespace api_authentication_boberto.Integrations.SMSAdbTester
{
    public static class SmsADBTesterApiBuilder
    {
        public static void BuildADBTesterBuilder(this IServiceCollection services, IConfigurationRoot config)
        {
            var configOptions = config.GetSection("SmsAdbTesterApiConfig").Get<SmsAdbTesterApiConfig>();

            ISmsAdbTesterApi api = RestClient.For<ISmsAdbTesterApi>(configOptions.Url);

            services.AddSingleton(api);
        }
    }
}
