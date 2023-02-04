using System.Security.Claims;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.Services.ApiKeyAuthentication.Models
{
    public class GetApiKeyModel
    {
        public string ApiKey { get; set; }
        public string ApiKeyCrypt { get; set; }
        public string ApiKeyHashed { get; set; }
        public string[] Scopes { get; set; }
        public int UserId { get; set; }

        public IEnumerable<Claim> GetClaims()
        {
            var claims = Scopes.Select(scope => new Claim("api_key_scope", scope));
            return claims;
        }
    }
}
