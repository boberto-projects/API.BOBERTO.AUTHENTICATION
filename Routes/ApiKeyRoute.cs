using api_authentication_boberto.CustomDbContext;
using api_authentication_boberto.EncryptionDecryptionUsingSymmetricKey;
using api_authentication_boberto.Interfaces;
using api_authentication_boberto.Models;
using api_authentication_boberto.Models.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NuGet.Configuration;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Data.Entity;
using System.Security.Cryptography;
using BC = BCrypt.Net.BCrypt;

namespace api_authentication_boberto.Routes
{
    public static class ApiKeyRoute
    {
        //const string _prefix = "MODEDITOR-";
        const int _numberOfSecureBytesToGenerate = 32;
        const int _lengthOfKey = 36;
        public static void AddApiKeyRoute(this WebApplication app)
        {
            //THE API KEY IS SHOW ONLY ONE TIME TO THE USER. THE USER HAS THE ORIGINAL API KEY AND WE STORE ONLY THE HASH VALUE IN DB
            app.MapPost("/apikey/generate", [Authorize] (
                [FromServices] DatabaseContext dbContext,
                [FromServices] IUsuarioService usuarioLogado,
                IOptions<ApiConfig> apiConfig) =>
                {
                    var usuario = usuarioLogado.ObterUsuarioLogado();
                    var prefix = $"MODEDITOR-";
                    var bytes = RandomNumberGenerator.GetBytes(_numberOfSecureBytesToGenerate);

                    var normalKey = string.Concat(prefix, Convert.ToBase64String(bytes)
                        .Replace("/", "")
                        .Replace("+", "")
                        .Replace("=", "")
                        .AsSpan(0, _lengthOfKey - prefix.Length));

                    var hashKey = BC.HashPassword(normalKey);
                    var apiKeys = dbContext.ApiKey.Where(e => e.UsuarioId.Equals(usuario.Id)).ToList();
                    var apiKey = new ApiKeyModel()
                    {
                        ApiKey = hashKey,
                        UsuarioId = usuario.Id,
                        Scopes = new List<string>() { "manage_modpack" }

                    };
                    if (apiKeys == null)
                    {
                        return "USER NOT FOUND???? WTF";
                    }
                    var user = dbContext.Usuarios.FirstOrDefault(e => e.UsuarioId.Equals(usuario.Id));
                    if (apiKeys.Count() == 0)
                    {
                        user.ApiKeys = new List<ApiKeyModel>();
                        user.ApiKeys.Add(apiKey);
                    }
                    dbContext.SaveChanges();
                    return $"{normalKey}";
                }).WithTags("Api Key");

        }
    }
}
