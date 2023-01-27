using api_authentication_boberto.Domain.CustomDbContext;
using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Services.ApiKeyAuthentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace api_authentication_boberto.Authentications
{
    public class UserApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private ApiConfig ApiConfig { get; set; }
        private IApiKeyAuthenticationService ApiKeyService { get; set; }
        public UserApiKeyAuthenticationHandler(IOptions<ApiConfig> _apiConfig,
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            IApiKeyAuthenticationService apiKeyService,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            ApiConfig = _apiConfig.Value;
            ApiKeyService = apiKeyService;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            ///TODO: this is a base to use before identity class.

            if (Request.Headers.TryGetValue(ApiConfig.Authorization.ApiHeader, out
               var extractedApiKey) == false)
            {
                return Task.FromResult(AuthenticateResult.Fail("Api key not found."));
            }
            var apiKeyInvalid = ApiKeyService.IsValid() == false;
            var apiKey = ApiKeyService.Get(extractedApiKey);
            if (apiKeyInvalid || apiKey == null)
            {
                return Task.FromResult(AuthenticateResult.Fail("Api key wrong or not exists."));
            }
            var claims = apiKey.GetClaims();
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }


}
