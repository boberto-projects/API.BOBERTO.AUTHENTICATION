using api_authentication_boberto.Models.Config;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace api_authentication_boberto.Services.Implements
{
    public class AtualizarAppsettings
    {
        private string arquivoAppSettings => "appsettings.json";
        public void AtualizarResource(IEnumerable<ResourceOptionConfig> resourceSettings)
        {
            var appSettingsJson = File.ReadAllText(arquivoAppSettings, Encoding.UTF8);
            var jsonResponse = JsonConvert.DeserializeObject<AppSettingsConfig>(appSettingsJson);
            jsonResponse.ResourcesConfig.Resources = resourceSettings;
            var json = JsonConvert.SerializeObject(jsonResponse, Formatting.Indented);
            File.WriteAllText(arquivoAppSettings, json, encoding: Encoding.UTF8);
        } 
    }
}
