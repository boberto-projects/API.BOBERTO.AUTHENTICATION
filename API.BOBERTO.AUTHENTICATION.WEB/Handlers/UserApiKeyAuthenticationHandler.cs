using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Config;
using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions;
using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions.Models;
using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.ApiKeyAuthentication;
using api_authentication_boberto.API.BOBERTO.AUTHENTICATION.APPLICATION.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace API.BOBERTO.AUTHENTICATION.WEB.Handlers
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
            if (Request.Headers.TryGetValue(ApiConfig.AuthorizationConfig.ApiHeader, out
               var extractedApiKey) == false)
            {
                throw new ApiKeyAuthenticationException(ExceptionTypeEnum.AUTHORIZATION);
            }
            var apiKeyInvalid = EncryptUtils.IsBase64(extractedApiKey) == false;
            if (apiKeyInvalid)
            {
                throw new ApiKeyAuthenticationException(ExceptionTypeEnum.AUTHORIZATION);
            }
            var apiKey = ApiKeyService.Get(extractedApiKey);
            if (apiKey == null)
            {
                throw new ApiKeyAuthenticationException(ExceptionTypeEnum.AUTHORIZATION);
            }
            var claims = apiKey.GetClaims();
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }


}
