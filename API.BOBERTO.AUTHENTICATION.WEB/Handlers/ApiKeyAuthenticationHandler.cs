using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Config;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace API.BOBERTO.AUTHENTICATION.WEB.Handlers
{

    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private IOptions<ApiConfig> _apiConfig { get; set; }
        public ApiKeyAuthenticationHandler(IOptions<ApiConfig> apiConfig, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _apiConfig = apiConfig;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "LOGADO por API KEY") };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            if (_apiConfig.Value.AuthorizationConfig.Enabled == false)
            {
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }

            if (Request.Headers.TryGetValue(_apiConfig.Value.ApiKeyAuthenticationConfig.ApiHeader, out
                   var extractedApiKey) == false)
            {
                return Task.FromResult(AuthenticateResult.Fail("Api key não informada"));
            }

            if (_apiConfig.Value.AuthorizationConfig.ApiKey.Equals(extractedApiKey) == false)
            {
                return Task.FromResult(AuthenticateResult.Fail("Não autorizado"));
            }

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

}
