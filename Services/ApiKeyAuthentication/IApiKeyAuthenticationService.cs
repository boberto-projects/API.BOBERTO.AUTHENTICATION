using api_authentication_boberto.Models;

namespace api_authentication_boberto.Services.ApiKeyAuthenticationService
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
