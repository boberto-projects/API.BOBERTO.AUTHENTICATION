using api_authentication_boberto.Domain.CustomDbContext;
using api_authentication_boberto.Exceptions;
using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Models.Enums;
using api_authentication_boberto.Services.ApiKeyAuthentication;
using api_authentication_boberto.Services.CurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


namespace API.BOBERTO.AUTHENTICATION.WEB.Routes
{
    public static class ApiKeyRoute
    {

        public static void AddApiKeyRoute(this WebApplication app)
        {
            app.MapPost("/apikey/generate", [Authorize] (
                [FromServices] IApiKeyAuthenticationService apiKeyService,
                [FromServices] DatabaseContext dbContext,
                [FromServices] ICurrentUserService currentUser,
                IOptions<ApiConfig> apiConfig) =>
                {
                    var user = currentUser.ObterUsuarioLogado();
                    if (user.Role == RolesEnum.USER)
                    {
                        throw new CustomException(StatusCodeEnum.BUSINESS, "YOU CANT USE THIS");
                    }
                    var key = apiKeyService.Generate(user.Id, user.Role);
                    var apiKeys = dbContext.ApiKey.Where(e => e.UserId.Equals(user.Id)).ToList();
                    var userContext = dbContext.Usuarios.FirstOrDefault(e => e.UserId.Equals(user.Id));
                    if (apiKeys.Count() > 0)
                    {
                        throw new CustomException(StatusCodeEnum.BUSINESS, "You already have a key.");
                    }
                    var apiKey = new ApiKeyModel()
                    {
                        ApiKey = key.ApiKeyCrypt,
                        UserId = user.Id,
                    };
                    apiKey.AddScopes(key.Scopes);
                    userContext.ApiKeys.Add(apiKey);
                    dbContext.SaveChanges();
                    return $"{key.ApiKeyHashed}";
                }).WithTags("User Api Key");

            app.MapPost("/apikey/revoke", [Authorize] (
                [FromServices] IApiKeyAuthenticationService apiKeyService,
                [FromServices] DatabaseContext dbContext,
                [FromServices] ICurrentUserService currentUser,
                IOptions<ApiConfig> apiConfig) =>
            {
                var user = currentUser.ObterUsuarioLogado();
                if (user.Role == RolesEnum.USER)
                {
                    throw new CustomException(StatusCodeEnum.BUSINESS, "YOU CANT USE THIS");
                }
                var apiKeys = dbContext.ApiKey.Where(e => e.UserId.Equals(user.Id)).ToList();
                if (apiKeys.Count() == 0)
                {
                    throw new CustomException(StatusCodeEnum.BUSINESS, "You need to generate a api key first.");
                }
                var key = apiKeyService.Generate(user.Id, user.Role);
                var userContext = dbContext.Usuarios.FirstOrDefault(e => e.UserId.Equals(user.Id));
                userContext.ApiKeys.Clear();
                var apiKey = new ApiKeyModel()
                {
                    ApiKey = key.ApiKeyCrypt,
                    UserId = user.Id,
                };
                apiKey.AddScopes(key.Scopes);
                userContext.ApiKeys.Add(apiKey);
                dbContext.SaveChanges();
                return $"{key.ApiKeyHashed}";
            }).WithTags("User Api Key");

        }
    }
}
