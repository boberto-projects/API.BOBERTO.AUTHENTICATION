using api_authentication_boberto.Services.Redis;

namespace api_authentication_boberto.DependencyInjection
{
    public static partial class DependencyInjection
    {
        public static void AddRedisStorage(this WebApplicationBuilder builder)
        {
            var serviceProvider = builder.Services.BuildServiceProvider();
            var config = serviceProvider.GetRequiredService<IConfigurationRoot>();
            var redisContext = config.GetConnectionString("RedisConnectionContext");
            builder.Services.AddStackExchangeRedisCache(options => options.Configuration = ObterRedisContext(redisContext));
            builder.Services.AddSingleton<IRedisService, RedisService>();
        }
        private static string ObterRedisContext(string redisContextUrl)
        {
            Uri redisUrl;
            bool isRedisUrl = Uri.TryCreate(redisContextUrl, UriKind.Absolute, out redisUrl);
            if (isRedisUrl)
            {
                redisContextUrl = string.Format("{0}:{1},password={2}", redisUrl.Host, redisUrl.Port, redisUrl.UserInfo.Split(':')[1]);
            }
            return redisContextUrl;
        }
    }
}
