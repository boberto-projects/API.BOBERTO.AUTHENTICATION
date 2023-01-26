using api_authentication_boberto.Services.ApiKeyAuthenticationService;
using api_authentication_boberto.Services.GlobalConfig;
using api_authentication_boberto.Services.Implements;
using api_authentication_boberto.Services.OTP;
using api_authentication_boberto.Services.OTPSender;
using api_authentication_boberto.Services.RedisService;
using api_authentication_boberto.Services.SenderService;
using api_authentication_boberto.Services.User;

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
            builder.Services.AddSingleton<AtualizarAppsettings>();
            builder.Services.AddSingleton<ZenvioSecurity>();
            builder.Services.AddSingleton<UserSecurity>();

            builder.Services.AddScoped<JWTService>();
            builder.Services.AddScoped<ICurrentUserService, UsuarioService>();

            builder.Services.AddScoped<IApiKeyAuthenticationService, ApiKeyAuthenticationService>();

            //config temporaria postgree
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
    }
}
