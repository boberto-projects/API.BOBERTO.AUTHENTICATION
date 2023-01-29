using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.ApiKeyAuthentication.Models;
using api_authentication_boberto.Models.Enums;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.Services.ApiKeyAuthentication
{
    public interface IApiKeyAuthenticationService
    {
        bool Validate(string key);
        ApiKeyModel Get(string key);
        string DecryptApiKey(string key);
        string EncryptApiKey(string key);
        ApiKeyModel Generate(int userId, RolesEnum role);
    }
}
