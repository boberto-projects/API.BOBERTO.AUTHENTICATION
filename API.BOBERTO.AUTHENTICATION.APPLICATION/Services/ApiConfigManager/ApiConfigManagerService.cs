using api_authentication_boberto.Models.Config;
using Newtonsoft.Json;
using System.Text;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.Services.ApiConfigManager
{
    public class ApiConfigManagerService
    {
        private string configFile => "appsettings.json";
        public void AtualizarResource(IEnumerable<ResourceOptionConfig> resourceSettings)
        {
            var appSettingsJson = File.ReadAllText(configFile, Encoding.UTF8);
            var jsonResponse = JsonConvert.DeserializeObject<AppSettingsConfig>(appSettingsJson);
            jsonResponse.ResourcesConfig.Resources = resourceSettings;
            var json = JsonConvert.SerializeObject(jsonResponse, Formatting.Indented);
            File.WriteAllText(configFile, json, encoding: Encoding.UTF8);
        }
    }
}
