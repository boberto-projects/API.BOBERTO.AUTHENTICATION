using api_authentication_boberto.Domain.CustomDbContext;
using api_authentication_boberto.Implements;
using api_authentication_boberto.Integrations.DiscordApiClient;
using api_authentication_boberto.Integrations.SMSAdbTester;
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
using StackExchange.Redis;
using System.Configuration;
using System.Text;

namespace api_authentication_boberto
{
    public static class DependencyInjection
    {
        public static void InjetarServicos(this WebApplicationBuilder builder, IConfigurationRoot config)
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

        public static void InjetarConfiguracoes(this WebApplicationBuilder builder, IConfigurationRoot config)
        {
            builder.Services.Configure<GerenciadorAutenticacaoConfig>(options => config.GetSection("GerenciadorAutenticacaoConfig").Bind(options));
            builder.Services.Configure<GerenciadorZenvioConfig>(options => config.GetSection("GerenciadorZenvioConfig").Bind(options));
            builder.Services.Configure<DiscordAPIConfig>(options => config.GetSection("DiscordApiConfig").Bind(options));
            builder.Services.Configure<TwoFactorConfig>(options => config.GetSection("TwoFactorConfig").Bind(options));
            builder.Services.Configure<ZenviaApiConfig>(options => config.GetSection("ZenviaApiConfig").Bind(options));
            builder.Services.Configure<ApiConfig>(options => config.GetSection("ApiConfig").Bind(options));
            builder.Services.Configure<SmtpConfig>(options => config.GetSection("SmtpConfig").Bind(options));
            builder.Services.Configure<ResourcesConfig>(options => config.GetSection("ResourcesConfig").Bind(options));
            builder.Services.Configure<JwtConfig>(options => config.GetSection("JwtConfig").Bind(options));
        }

        public static void InjetarServicosDeArmazenamento(this WebApplicationBuilder builder, IConfigurationRoot config)
        {
            ///O plugin postgree do dokku insere uma URL na variavel de ambiente. Mas localmente não usamos link direto com o POSTGREE.
            ///GAMB pra converter uma url para o esquema de autenticação da biblioteca Npgsql
            builder.Services.AddEntityFrameworkNpgsql().AddDbContext<DatabaseContext>(o => o.UseNpgsql(ObterPostGreeContext()));
            ///redis               
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

            string ObterPostGreeContext()
            {
                var postGreeContext = config.GetConnectionString("PostgreeConnectionContext");
                var postGreeConnectionBuilder = new NpgsqlConnectionStringBuilder();
                Uri postGreeUrl;
                bool isPostGreeUrl = Uri.TryCreate(postGreeContext, UriKind.Absolute, out postGreeUrl);
                if (isPostGreeUrl)
                {
                    postGreeConnectionBuilder.Host = postGreeUrl.Host;
                    postGreeConnectionBuilder.Port = postGreeUrl.Port;
                    postGreeConnectionBuilder.Database = postGreeUrl.LocalPath.Substring(1);
                    postGreeConnectionBuilder.Username = postGreeUrl.UserInfo.Split(':')[0];
                    postGreeConnectionBuilder.Password = postGreeUrl.UserInfo.Split(':')[1];
                    postGreeContext = postGreeConnectionBuilder.ToString();
                }
                return postGreeContext;
            }
        }

        public static void InjetarIntegracoes(this WebApplicationBuilder builder, IConfigurationRoot config)
        {
            builder.Services.BuildZenviaAPI(config);
            builder.Services.BuildDiscordAPI(config);
            builder.Services.BuildADBTesterBuilder(config);

            builder.Services.AddSingleton<IEmailService, EmailService>();
            builder.Services.AddSingleton<DiscordService>();
            builder.Services.AddSingleton<ZenvioService>();
        }

        public static void InjetarServicosAutenticacao(this WebApplicationBuilder builder, IConfigurationRoot config)
        {
            var jwtKey = Encoding.ASCII.GetBytes(config["JwtConfig:Key"]);
            builder.Services.AddAuthentication("ApiKeyAuthenticationHandler")
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>
            ("ApiKeyAuthenticationHandler", null);
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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
