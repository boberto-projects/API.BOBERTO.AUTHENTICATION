using api_authentication_boberto.Models;
using api_authentication_boberto.Models.Enums;

namespace api_authentication_boberto.Services.ApiKeyAuthentication
{
    public interface IApiKeyAuthenticationService
    {
         bool Validate(string key);
        bool IsValid();
         GetApiKeyModel Get(string key);
         string DecryptApiKey(string key);
         string EncryptApiKey(string key);
         GetApiKeyModel Generate(int userId);
         GetApiKeyModel Generate(int userId, RolesEnum role);

    }
}
