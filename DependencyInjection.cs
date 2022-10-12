using api_authentication_boberto.CustomDbContext;
using api_authentication_boberto.Implements;
using api_authentication_boberto.Integrations.DiscordApiClient;
using api_authentication_boberto.Integrations.ZenviaApiClient;
using api_authentication_boberto.Interfaces;
using api_authentication_boberto.Models;
using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Services.Implements;
using api_authentication_boberto.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Text;

namespace api_authentication_boberto
{
    public static class DependencyInjection
    {
        public static void InjetarServicos(this WebApplicationBuilder builder, IConfigurationRoot config)
        {

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSingleton<IRedisService, RedisService>();
            builder.Services.AddSingleton<IOTPCode, OTPCodeService>();

            builder.Services.AddScoped<IUsuarioService, UsuarioService>();
            builder.Services.AddScoped<IEnviarCodigoDuploFator, EnviarCodigoDuploFator>();

            builder.Services.AddSingleton<ApiCicloDeVida>();


            builder.Services.AddScoped<GerenciadorAutenticacao>();
            builder.Services.AddScoped<GerenciadorZenvio>();
        }

        public static void InjetarConfiguracoes(this WebApplicationBuilder builder, IConfigurationRoot config)
        {
            builder.Services.Configure<GerenciadorAutenticacaoConfig>(options => config.GetSection("GerenciadorAutenticacaoConfig").Bind(options));
            builder.Services.Configure<GerenciadorZenvioConfig>(options => config.GetSection("GerenciadorZenvioConfig").Bind(options));
            builder.Services.Configure<DiscordAPIConfig>(options => config.GetSection("DiscordApiConfig").Bind(options));
            builder.Services.Configure<TwoFactorConfig>(options => config.GetSection("TwoFactorConfig").Bind(options));
            builder.Services.Configure<ZenviaApiConfig>(options => config.GetSection("ZenviaApiConfig").Bind(options));
            builder.Services.Configure<ApiConfig>(options => config.GetSection("ApiConfig").Bind(options));
            builder.Services.Configure<SmtpConfig>(options => config.GetSection("SmtpConfig").Bind(options));

        }

        public static void InjetarServicosDeArmazenamento(this WebApplicationBuilder builder, IConfigurationRoot config)
        {
            ///O plugin postgree do dokku pode inserir uma URL na variavel de ambiente. Mas localmente não usamos link direto com o POSTGREE.
            ///GAMB pra converter uma url para o esquema de autenticação da biblioteca Npgsql
            var contextUrl = config.GetConnectionString("Postgree");
            var postGreeConnectionBuilder = new NpgsqlConnectionStringBuilder();
            Uri url;
            bool isUrl = Uri.TryCreate(contextUrl, UriKind.Absolute, out url);
            if (isUrl)
            {
                postGreeConnectionBuilder.Host = url.Host;
                postGreeConnectionBuilder.Port = url.Port;
                postGreeConnectionBuilder.Database = url.LocalPath.Substring(1);
                postGreeConnectionBuilder.Username = url.UserInfo.Split(':')[0];
                postGreeConnectionBuilder.Password = url.UserInfo.Split(':')[1];
                builder.Services.AddEntityFrameworkNpgsql().AddDbContext<DatabaseContext>(o => o.UseNpgsql(postGreeConnectionBuilder.ToString()));
            }

            builder.Services.AddEntityFrameworkNpgsql().AddDbContext<DatabaseContext>(o => o.UseNpgsql(contextUrl));
            ///redis
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config.GetConnectionString("Redis");
            });
        }

        public static void InjetarIntegracoes(this WebApplicationBuilder builder, IConfigurationRoot config)
        {
            builder.Services.BuildZenviaAPI(config);
            builder.Services.BuildDiscordAPI(config);

            builder.Services.AddSingleton<IEmailService, EmailService>();
            builder.Services.AddSingleton<DiscordService>();
            builder.Services.AddSingleton<ZenvioService>();
        }

        public static void InjetarServicosAutenticacao(this WebApplicationBuilder builder, IConfigurationRoot config)
        {
            var jwtKey = Encoding.ASCII.GetBytes(config["Jwt:Key"]);


            builder.Services.AddAuthentication("ApiKeyAuthenticationHandler")
                   .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>
                   ("ApiKeyAuthenticationHandler", null);

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }
    }
}
