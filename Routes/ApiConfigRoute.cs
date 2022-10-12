using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace api_authentication_boberto.Routes
{
    public static class ApiConfigRoute
    {
        public static void AdicionarApiConfigRoute(this WebApplication app)
        {
            //obter status dos serviços de integração e uso de sms
            app.MapGet("/status", [Authorize(AuthenticationSchemes = "ApiKeyAuthenticationHandler")] () =>
            {

            });
            
        }

          
    }
}
