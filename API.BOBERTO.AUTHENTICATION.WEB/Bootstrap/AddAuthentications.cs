﻿using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Config;
using API.BOBERTO.AUTHENTICATION.WEB.Handlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.AUTHENTICATION.BOBERTO.WEB.Bootstrap
{
    public static partial class Bootstrap
    {
        public static void AddAuthentications(this WebApplicationBuilder builder)
        {
            var serviceProvider = builder.Services.BuildServiceProvider();
            var jwtConfig = serviceProvider.GetRequiredService<IOptions<JwtConfig>>();

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

            var jwtKey = Encoding.ASCII.GetBytes(jwtConfig.Value.Key);
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
