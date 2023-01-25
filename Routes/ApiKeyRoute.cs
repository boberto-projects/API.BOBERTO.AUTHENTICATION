using api_authentication_boberto.Domain.CustomDbContext;
using api_authentication_boberto.EncryptionDecryptionUsingSymmetricKey;
using api_authentication_boberto.Interfaces;
using api_authentication_boberto.Models;
using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NuGet.Configuration;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Cryptography;
using BC = BCrypt.Net.BCrypt;

namespace api_authentication_boberto.Routes
{
    public static class ApiKeyRoute
    {

        public static void AddApiKeyRoute(this WebApplication app)
        {
            app.MapPost("/apikey/generate", [Authorize] (
                [FromServices] IApiKeyService apiKeyService,
                [FromServices] DatabaseContext dbContext,
                [FromServices] IUsuarioService usuarioLogado,
                IOptions<ApiConfig> apiConfig) =>
                {
                    var usuario = usuarioLogado.ObterUsuarioLogado();
                    var key = apiKeyService.GetApiKey();
                    var apiKeys = dbContext.ApiKey.Where(e => e.UsuarioId.Equals(usuario.Id)).ToList();
                    var user = dbContext.Usuarios.FirstOrDefault(e => e.UsuarioId.Equals(usuario.Id));
                    var apiKey = new ApiKeyModel()
                    {
                        ApiKey = key.ApiKeyHashed,
                        UsuarioId = usuario.Id,
                    };
                    apiKey.AddScopes(key.Scopes);
                    if (apiKeys.Count() > 0)
                    {
                        return "You already have a key.";
                    }
                    user.ApiKeys = new List<ApiKeyModel>();
                    user.ApiKeys.Add(apiKey);
                    dbContext.SaveChanges();
                    return $"{key.ApiKey}";
                }).WithTags("Api Key");

        }
    }
}
