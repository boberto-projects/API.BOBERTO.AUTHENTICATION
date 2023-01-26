using api_authentication_boberto.Models.Config;
using Newtonsoft.Json;
using System.Text;

namespace api_authentication_boberto.Services.GlobalConfig
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
