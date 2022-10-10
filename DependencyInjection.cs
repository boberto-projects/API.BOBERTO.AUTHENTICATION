using api_authentication_boberto.CustomDbContext;
using api_authentication_boberto.Implements;
using api_authentication_boberto.Interfaces;
using api_authentication_boberto.Models.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MinecraftServer.Api.Services;
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
            builder.Services.AddScoped<IUsuarioService, UsuarioService>();
        }

        public static void InjetarConfiguracoes(this WebApplicationBuilder builder, IConfigurationRoot config)
        {
            builder.Services.Configure<DiscordAPIConfig>(options => config.GetSection("DiscordApiConfig").Bind(options));
            builder.Services.Configure<TwoFactorConfig>(options => config.GetSection("TwoFactorConfig").Bind(options));
        }

        public static void InjetarServicosDeArmazenamento(this WebApplicationBuilder builder, IConfigurationRoot config)
        {
            ///postgree
            builder.Services.AddEntityFrameworkNpgsql().AddDbContext<DatabaseContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("Postgree")));

            ///redis
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config.GetConnectionString("Redis");
            });
        }

        public static void InjetarIntegracoes(this WebApplicationBuilder builder, IConfigurationRoot config)
        {

        }

        public static void InjetarServicosAutenticacao(this WebApplicationBuilder builder, IConfigurationRoot config)
        {
            var jwtKey = Encoding.ASCII.GetBytes(config["Jwt:Key"]);

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
