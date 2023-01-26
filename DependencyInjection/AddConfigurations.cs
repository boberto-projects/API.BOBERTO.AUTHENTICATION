using api_authentication_boberto.Models.Config;
using ConfigurationSubstitution;

namespace api_authentication_boberto.DependencyInjection
{
    public static partial class DependencyInjection
    {
        public static void AddConfigurations(this WebApplicationBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .EnableSubstitutions("%", "%")
                .Build();
            builder.Services.Configure<UserSecurityConfig>(options => config.GetSection("AuthenticationSecurityConfig").Bind(options));
            builder.Services.Configure<ZenvioSecurityConfig>(options => config.GetSection("ZenvioSecurityServiceConfig").Bind(options));
            builder.Services.Configure<DiscordAPIConfig>(options => config.GetSection("DiscordApiConfig").Bind(options));
            builder.Services.Configure<TwoFactorConfig>(options => config.GetSection("TwoFactorConfig").Bind(options));
            builder.Services.Configure<ZenviaApiConfig>(options => config.GetSection("ZenviaApiConfig").Bind(options));
            builder.Services.Configure<ApiConfig>(options => config.GetSection("ApiConfig").Bind(options));
            builder.Services.Configure<SmtpConfig>(options => config.GetSection("SmtpConfig").Bind(options));
            builder.Services.Configure<ResourcesConfig>(options => config.GetSection("ResourcesConfig").Bind(options));
            builder.Services.Configure<JwtConfig>(options => config.GetSection("JwtConfig").Bind(options));
        }
    }
}
