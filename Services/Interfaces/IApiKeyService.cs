using api_authentication_boberto.Domain.CustomDbContext;
using api_authentication_boberto.Models;

namespace api_authentication_boberto.Services.Interfaces
{
    public interface IApiKeyService
    {
        public bool Validate(string key);
        public GetApiKeyModel Get(string key);
        public string DecryptApiKey(string key);
        public string EncryptApiKey(string key);
        public GetApiKeyModel Generate();
    }
}
