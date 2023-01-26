using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Models.Request;
using api_authentication_boberto.Services.GlobalConfig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace api_authentication_boberto.Routes
{
    public static class ApiConfigRoute
    {
        public static void AdicionarApiConfigRoute(this WebApplication app)
        {
            //obter status dos serviços de integração e uso de sms
            app.MapGet("/apiconfig/resource", [Authorize(AuthenticationSchemes = "ApiKeyAuthenticationHandler")] (IOptions<ResourcesConfig> resourceConfig) =>
            {
                return resourceConfig;
            }).WithTags("Gerenciador de appsettings"); ;

            app.MapPost("/apiconfig/resource/atualizar", [Authorize(AuthenticationSchemes = "ApiKeyAuthenticationHandler")] ([FromBody] AlterarApiConfigRequest request, [FromServices] ApiConfigService atualizarAppSettings, IOptions<ResourcesConfig> resourceConfig) =>
            {
                var resources = resourceConfig.Value.Resources.ToList();

                resources.ForEach(
                resource =>
                {
                    if (resource.Key.Equals("PreferirDiscordAoSMS"))
                    {
                        resource.Enabled = request.PreferirDiscordAoSMS;
                    }
                });

                atualizarAppSettings.AtualizarResource(resources);
                return new
                {
                    PreferirDiscordAoSMS = request.PreferirDiscordAoSMS
                };
            }).WithTags("Gerenciador de appsettings");

        }
    }
}
