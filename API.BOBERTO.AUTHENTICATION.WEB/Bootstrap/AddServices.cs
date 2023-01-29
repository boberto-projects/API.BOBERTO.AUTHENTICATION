using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.ApiConfigManager;
using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.ApiKeyAuthentication;
using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.CurrentUser;
using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.JWT;
using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.OTP;
using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.OTPSender;
using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.UserSecurity;
using API.BOBERTO.AUTHENTICATION.WEB;

namespace API.AUTHENTICATION.BOBERTO.WEB.Bootstrap
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

            builder.Services.AddSingleton<HealthCheck>();
            builder.Services.AddSingleton<IOTPService, OTPService>();
            builder.Services.AddSingleton<IOTPSender, OTPSender>();
            builder.Services.AddSingleton<ApiConfigManagerService>();
            builder.Services.AddSingleton<IUserSecurity, UserSecurity>();
            builder.Services.AddScoped<IJWTService, JWTService>();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<IApiKeyAuthenticationService, ApiKeyAuthenticationService>();
        }
    }
}
