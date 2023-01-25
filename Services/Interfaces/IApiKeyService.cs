using api_authentication_boberto.Domain.CustomDbContext;
using api_authentication_boberto.Models;

namespace api_authentication_boberto.Services.Interfaces
{
    public interface IApiKeyService
    {
        public GetApiKeyModel? Validate(string key);
        public GetApiKeyModel GetApiKey();
    }
}
