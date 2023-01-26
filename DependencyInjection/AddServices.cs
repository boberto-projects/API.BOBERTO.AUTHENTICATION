using api_authentication_boberto.Services.ApiConfigManager;
using api_authentication_boberto.Services.ApiKeyAuthentication;
using api_authentication_boberto.Services.CurrentUser;
using api_authentication_boberto.Services.JWT;
using api_authentication_boberto.Services.OTP;
using api_authentication_boberto.Services.OTPSender;
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
            builder.Services.AddSingleton<IOTPService, OTPService>();
            builder.Services.AddSingleton<IOTPSender, OTPSender>();
            builder.Services.AddSingleton<ApiConfigManagerService>();
            builder.Services.AddSingleton<IZenvioSecurityService, ZenvioSecurityService>();
            builder.Services.AddSingleton<IUserSecurity, UserSecurity>();

            builder.Services.AddScoped<IJWTService, JWTService>();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<IApiKeyAuthenticationService, ApiKeyAuthenticationService>();
        }
    }
}
