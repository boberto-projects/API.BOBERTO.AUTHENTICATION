using api_authentication_boberto.Domain.CustomDbContext;
using api_authentication_boberto.EncryptionDecryptionUsingSymmetricKey;
using api_authentication_boberto.Exceptions;
using api_authentication_boberto.Models;
using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Models.Enums;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using BC = BCrypt.Net.BCrypt;

namespace api_authentication_boberto.Services.ApiKeyAuthentication
{
    public class ApiKeyAuthenticationService : IApiKeyAuthenticationService
    {
        const string _prefix = "BOBERTOAUTH-";
        const string _genericMessage = "INVALID API KEY";
        const int _numberOfSecureBytesToGenerate = 32;
        const int _lengthOfKey = 36;

        /// <summary>
        /// Scopes needs be enums.
        /// </summary>
        public static string[] DefaultScopes = new string[] { "manage_modpack" };
        DatabaseContext DbContext { get; set; }
        ApiConfig ApiConfig { get; set; }
        public ApiKeyAuthenticationService(DatabaseContext dbContext, IOptions<ApiConfig> apiConfig)
        {
            DbContext = dbContext;
            ApiConfig = apiConfig.Value;
        }
        /// <summary>
        /// Decrypt a key with a Aes secretKey
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string DecryptApiKey(string key)
        {
            return EncryptUtils.DecryptString(ApiConfig.ApiKeyAuthentication.Key, key);
        }
        /// <summary>
        /// Encrypt a key with a Aes secretKey
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string EncryptApiKey(string key)
        {
            return EncryptUtils.EncryptString(ApiConfig.ApiKeyAuthentication.Key, key);
        }
        /// <summary>
        /// Create a prefix. This is needed to user identifies what api key is it
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private string CreatePrefix(int userId) => string.Concat(_prefix, userId, "-");
        /// <summary>
        /// Create a Key prefix. This is needed to user identifies what api key is it
        /// </summary>
        /// <param name="key"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private string CreateKeyPrefix(string key, int userId) => string.Concat(CreatePrefix(userId), key);
        /// <summary>
        /// Remove a prefix from key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string RemoveKeyPrefix(string key)
        {
            var keyFrags = key.Split("-");
            if (keyFrags.Count() != 3)
            {
                return key;
            }
            return keyFrags[2];
        }
        /// <summary>
        /// Read a key without decrypt and get user id.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="CustomException"></exception>
        int GetUserId(string key)
        {
            var userId = key.Split("-")[1];
            if (userId == null)
            {
                throw new CustomException(StatusCodeEnum.VALIDATION, _genericMessage);
            }
            int result;
            int.TryParse(userId, out result);
            return result;
        }
        /// <summary>
        /// Validate Api Key and verify if the pattern match. We need to get user id splitting api key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="CustomException"></exception>
        public bool Validate(string key)
        {
            var keyFrags = key.Split("-");
            if (keyFrags.Count() != 3)
            {
                throw new CustomException(StatusCodeEnum.VALIDATION, _genericMessage);
            }
            var keyWithoutPrefix = keyFrags.ElementAt(2);
            var decrypt = DecryptApiKey(keyWithoutPrefix);
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
                throw new CustomException(StatusCodeEnum.NOTAUTHORIZED, "Api Key invalid or expired.");
            }
            var keyWithoutPrefix = RemoveKeyPrefix(key);
            var decrypt = DecryptApiKey(keyWithoutPrefix);
            var userId = GetUserId(key);
            var apiKey = DbContext.ApiKey.FirstOrDefault(token => token.UserId.Equals(userId));
            if (apiKey != null)
            {
                var apiKeyVerified = BC.Verify(decrypt, apiKey.ApiKey);
                if (apiKeyVerified == false)
                {
                    throw new CustomException(StatusCodeEnum.NOTAUTHORIZED, "Api Key invalid or expired.");
                }
                return new GetApiKeyModel()
                {
                    ApiKey = decrypt,
                    Scopes = apiKey.Scopes.ToArray(),
                    UserId = apiKey.UserId
                };
            }
            return null;
        }

        /// <summary>
        /// Generate API KEY
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>GetApiKeyModel</returns>
        public GetApiKeyModel Generate(int userId)
        {
            var key = GenerateApiKey();
            var hashKey = EncryptApiKey(key);
            var cryptKey = BC.HashPassword(key);
            return new GetApiKeyModel()
            {
                ApiKey = key,
                ApiKeyHashed = CreateKeyPrefix(hashKey, userId),
                ApiKeyCrypt = cryptKey,
                Scopes = DefaultScopes
            };
        }
        /// <summary>
        /// Generate API KEY With Scopes foreach user Role
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>GetApiKeyModel</returns>
        public GetApiKeyModel Generate(int userId, RolesEnum role)
        {
            var key = GenerateApiKey();
            var hashKey = EncryptApiKey(key);
            var cryptKey = BC.HashPassword(key);
            var scopes = GetScopes(role);
            return new GetApiKeyModel()
            {
                ApiKey = key,
                ApiKeyHashed = CreateKeyPrefix(hashKey, userId),
                ApiKeyCrypt = cryptKey,
                Scopes = scopes.Select(x => x.ToString()).ToArray()
            };
        }
        /// <summary>
        /// Get Scopes from Role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        private List<ScopesEnum> GetScopes(RolesEnum role)
        {
            var scopes = new List<ScopesEnum>();
            switch (role)
            {
                case RolesEnum.MODPACK_CREATOR:
                    scopes.Add(ScopesEnum.MODPACK_CREATE);
                    scopes.Add(ScopesEnum.MODPACK_UPDATE);
                    scopes.Add(ScopesEnum.MODPACK_DELETE);
                    scopes.Add(ScopesEnum.MODPACK_EDIT);
                    break;
                case RolesEnum.SERVER_MANAGER:
                    scopes.AddRange(GetScopes(RolesEnum.MODPACK_CREATOR));
                    scopes.Add(ScopesEnum.SERVER_CREATE);
                    scopes.Add(ScopesEnum.SERVER_UPDATE);
                    scopes.Add(ScopesEnum.SERVER_DELETE);
                    scopes.Add(ScopesEnum.SERVER_EDIT);
                    break;
            }
            return scopes;
        }
        /// <summary>
        /// Generate Api Key Without Sufix
        /// </summary>
        /// <returns></returns>
        private string GenerateApiKey()
        {
            var bytes = RandomNumberGenerator.GetBytes(_numberOfSecureBytesToGenerate);
            var key = Convert.ToBase64String(bytes)
                .Replace("/", "")
                .Replace("+", "")
                .Replace("=", "");
            return key;
        }
    }
}
