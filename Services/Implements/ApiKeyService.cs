using api_authentication_boberto.Domain.CustomDbContext;
using api_authentication_boberto.EncryptionDecryptionUsingSymmetricKey;
using api_authentication_boberto.Exceptions;
using api_authentication_boberto.Models;
using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using BC = BCrypt.Net.BCrypt;

namespace api_authentication_boberto.Services.Implements
{
    public class ApiKeyService : IApiKeyService
    {
        const string _prefix = "BOBERTOAUTH-";
        const int _numberOfSecureBytesToGenerate = 32;
        const int _lengthOfKey = 36;

        /// <summary>
        /// Scopes needs be enums.
        /// </summary>
        public static string[] DefaultScopes = new string[] { "manage_modpack" };
        DatabaseContext DbContext { get; set; }
        ApiConfig ApiConfig { get; set; }
        public ApiKeyService(DatabaseContext dbContext, IOptions<ApiConfig> apiConfig)
        {
            DbContext = dbContext;
            ApiConfig = apiConfig.Value;
        }

        public string DecryptApiKey(string key)
        {
            return EncryptUtils.DecryptString(ApiConfig.ApiKeyAuthentication.Key, key);
        }
        public string EncryptApiKey(string key)
        {
            return EncryptUtils.EncryptString(ApiConfig.ApiKeyAuthentication.Key, key);
        }
        public bool Validate(string key)
        {
            var decrypt = DecryptApiKey(key);
            return decrypt != null;
        }
        /// <summary>
        /// Get a API by ApiKey
        /// I Put this api key with hashed string with key because its wrong that a user with invalid api key can do multiple requests do database
        /// This can be a issue. So.. we need to put this as cache on redis and validate the api key before acess user settings
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="CustomException"></exception>
        public GetApiKeyModel? Get(string key)
        {
            var isValid = Validate(key);
            if (isValid == false)
            {
                throw new CustomException(StatusCodeEnum.NaoAutorizado, "Api Key invalid or expired.");
            }
            var decrypt = DecryptApiKey(key);
            var apiKey = DbContext.ApiKey.FirstOrDefault(e => e.ApiKey.Equals(decrypt));
            if (apiKey != null)
            {
                var apiKeyVerified = BC.Verify(decrypt, apiKey.ApiKey);
                if (apiKeyVerified == false)
                {
                    throw new CustomException(StatusCodeEnum.NaoAutorizado, "Api Key invalid or expired.");
                }
                return new GetApiKeyModel()
                {
                    ApiKey = decrypt,
                    Scopes = apiKey.Scopes.ToArray(),
                    UserId = apiKey.UsuarioId
                };
            }
            return null;
        }

        /// <summary>
        /// Get a api key.
        /// </summary>
        /// <returns></returns>
        public GetApiKeyModel Generate()
        {
            var bytes = RandomNumberGenerator.GetBytes(_numberOfSecureBytesToGenerate);
            var key = string.Concat(_prefix, Convert.ToBase64String(bytes)
                .Replace("/", "")
                .Replace("+", "")
                .Replace("=", "")
                .AsSpan(0, _lengthOfKey - _prefix.Length));

            var hashAes = EncryptUtils.EncryptString(ApiConfig.ApiKeyAuthentication.Key, key);
            var hashKey = BC.HashPassword(key);
            return new GetApiKeyModel()
            {
                ApiKey = key,
                ApiKeyCrypt = hashAes,
                ApiKeyHashed = hashKey,
                Scopes = DefaultScopes
            };
        }
    }
}
