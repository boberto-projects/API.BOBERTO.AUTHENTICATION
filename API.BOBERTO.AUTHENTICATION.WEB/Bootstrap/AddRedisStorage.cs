
using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.Redis;

namespace API.AUTHENTICATION.BOBERTO.WEB.Bootstrap
{
    public static partial class DependencyInjection
    {
        public static void AddRedisStorage(this WebApplicationBuilder builder)
        {
            var serviceProvider = builder.Services.BuildServiceProvider();
            var config = serviceProvider.GetRequiredService<IConfigurationRoot>();
            var redisContext = config.GetConnectionString("RedisConnectionContext");
            builder.Services.AddStackExchangeRedisCache(options => options.Configuration = GetRedisContext(redisContext));
            builder.Services.AddSingleton<IRedisService, RedisService>();
        }
        private static string GetRedisContext(string redisContextUrl)
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
