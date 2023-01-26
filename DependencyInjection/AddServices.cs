using api_authentication_boberto.Services.ApiKeyAuthentication;
using api_authentication_boberto.Services.GlobalConfig;
using api_authentication_boberto.Services.JWT;
using api_authentication_boberto.Services.OTP;
using api_authentication_boberto.Services.OTPSender;
using api_authentication_boberto.Services.Redis;
using api_authentication_boberto.Services.SenderService;
using api_authentication_boberto.Services.User;
using api_authentication_boberto.Services.UserSecurity;
using api_authentication_boberto.Services.ZenvioSecurity;

namespace api_authentication_boberto.DependencyInjection
{
    public static partial class DependencyInjection
    {
        public static void AddServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddCors();

            builder.Services.AddAuthorization();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSingleton<ApiCicloDeVida>();
            builder.Services.AddSingleton<IRedisService, RedisService>();
            builder.Services.AddSingleton<IOTPService, OTPService>();
            builder.Services.AddSingleton<IOTPSender, OTPSender>();
            builder.Services.AddSingleton<ApiConfigService>();
            builder.Services.AddSingleton<IZenvioSecurityService, ZenvioSecurityService>();
            builder.Services.AddSingleton<IUserSecurity, UserSecurity>();

            builder.Services.AddScoped<IJWTService, JWTService>();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

            builder.Services.AddScoped<IApiKeyAuthenticationService, ApiKeyAuthenticationService>();

            //config temporaria postgree
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
    }
}
