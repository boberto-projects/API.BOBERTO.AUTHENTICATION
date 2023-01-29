using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Enums.Authentication;
using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.ApiKeyAuthentication.Models;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.Services.ApiKeyAuthentication
{
    public interface IApiKeyAuthenticationService
    {
        bool Validate(string key);
        GetApiKeyModel Get(string key);
        string DecryptApiKey(string key);
        string EncryptApiKey(string key);
        GetApiKeyModel Generate(int userId, RolesEnum role);
    }
}
