using api_authentication_boberto.Models;
using api_authentication_boberto.Models.Enums;

namespace api_authentication_boberto.Services.ApiKeyAuthentication
{
    public interface IApiKeyAuthenticationService
    {
        public bool Validate(string key);
        public GetApiKeyModel Get(string key);
        public string DecryptApiKey(string key);
        public string EncryptApiKey(string key);
        public GetApiKeyModel Generate(int userId);
        public GetApiKeyModel Generate(int userId, RolesEnum role);

    }
}
