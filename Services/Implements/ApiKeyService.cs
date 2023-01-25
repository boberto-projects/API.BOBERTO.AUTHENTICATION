using api_authentication_boberto.Domain.CustomDbContext;
using api_authentication_boberto.EncryptionDecryptionUsingSymmetricKey;
using api_authentication_boberto.Exceptions;
using api_authentication_boberto.Models;
using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using BC = BCrypt.Net.BCrypt;

namespace api_authentication_boberto.Services.Implements
{
    public class ApiKeyService : IApiKeyService
    {
        const string _prefix = "MODEDITOR-";
        const int _numberOfSecureBytesToGenerate = 32;
        const int _lengthOfKey = 36;
        public static string[] DefaultScopes = new string[] { "manage_modpack" };
        DatabaseContext DbContext { get; set; }
        ApiConfig ApiConfig { get; set; }
        public ApiKeyService(DatabaseContext dbContext, IOptions<ApiConfig> apiConfig)
        {
            DbContext = dbContext;
            ApiConfig = apiConfig.Value;
        }
        public GetApiKeyModel? Validate(string key)
        {
            ///Only valid if siugned by tis api
            var errors = new List<string>();
            try
            {
                var decrypt = EncryptUtils.DecryptString(ApiConfig.ApiKeyAuthentication.Key, key);
                var apiKeys = DbContext.ApiKey.FirstOrDefault(e => e.ApiKey.Equals(decrypt));

                if (apiKeys != null)
                {
                    var apiKeyVerified = BC.Verify(decrypt, apiKeys.ApiKey);
                    if (apiKeyVerified == false)
                    {
                        throw new CustomException(StatusCodeEnum.NaoAutorizado, "Api Key invalid or expired.");
                    }
                    return new GetApiKeyModel()
                    {
                        ApiKey = decrypt,
                        Scopes = apiKeys.Scopes.ToArray()
                    };
                }
            }

            catch (Exception ex)
            {
                throw new CustomException(StatusCodeEnum.NaoAutorizado, "You cant acess this route");
            }
            return null;
        }

        /// <summary>
        /// Get a api key.
        /// </summary>
        /// <returns></returns>
        public GetApiKeyModel GetApiKey()
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
                ApiKey = hashAes,
                ApiKeyHashed = hashKey,
                Scopes = DefaultScopes
            };
        }
    }
}
