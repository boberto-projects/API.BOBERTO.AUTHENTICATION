using api_authentication_boberto.Implements;
using api_authentication_boberto.Interfaces;
using api_authentication_boberto.Services.Implements;
using api_authentication_boberto.Services.Interfaces;

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
            builder.Services.AddSingleton<IOTPCode, OTPCodeService>();
            builder.Services.AddSingleton<IEnviarCodigoDuploFator, EnviarCodigoDuploFator>();
            builder.Services.AddSingleton<AtualizarAppsettings>();
            builder.Services.AddSingleton<GerenciadorZenvio>();
            builder.Services.AddSingleton<GerenciadorAutenticacao>();

            builder.Services.AddScoped<TokenJWTService>();
            builder.Services.AddScoped<IUsuarioService, UsuarioService>();

            builder.Services.AddScoped<IApiKeyService, ApiKeyService>();

            //config temporaria postgree
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
    }
}
