﻿using api_authentication_boberto.Domain.CustomDbContext;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace api_authentication_boberto.DependencyInjection
{
    public static partial class DependencyInjection
    {
        public static void AddPostgreeStorage(this WebApplicationBuilder builder)
        {
            var serviceProvider = builder.Services.BuildServiceProvider();
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            builder.Services.AddEntityFrameworkNpgsql().AddDbContext<DatabaseContext>(o => o.UseNpgsql(GetPostgreeContext()));
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            string GetPostgreeContext()
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
    }
}
