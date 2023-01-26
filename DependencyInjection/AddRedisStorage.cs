namespace api_authentication_boberto.DependencyInjection
{
    public static partial class DependencyInjection
    {
        public static void AddRedisStorage(this WebApplicationBuilder builder)
        {
            var config = builder.Configuration;
            builder.Services.AddStackExchangeRedisCache(options => options.Configuration = ObterRedisContext());
            string ObterRedisContext()
            {
                var redisContextUrl = config.GetConnectionString("RedisConnectionContext");
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
}
