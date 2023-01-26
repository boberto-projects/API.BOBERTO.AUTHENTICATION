using api_authentication_boberto.Authentications;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace api_authentication_boberto.DependencyInjection
{
    public static partial class DependencyInjection
    {
        public static void AddAuthentications(this WebApplicationBuilder builder)
        {
            var config = builder.Configuration;

            builder.Services.AddAuthentication("api_key")
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>
    ("api_key", null);

            builder.Services.AddAuthentication("user_api_key")
.AddScheme<AuthenticationSchemeOptions, UserApiKeyAuthenticationHandler>
("user_api_key", null);

            builder.Services.AddAuthorization(options =>
          options.AddPolicy("modpack_manage",
          policy => policy.RequireClaim("api_key_scope",
          "MODPACK_CREATE")));

            var jwtKey = Encoding.ASCII.GetBytes(config["JwtConfig:Key"]);

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
