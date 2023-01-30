using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Config;
using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Enums.Authentication;
using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions;
using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions.Models;
using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.ApiKeyAuthentication.Models;
using API.BOBERTO.AUTHENTICATION.DOMAIN;
using api_authentication_boberto.API.BOBERTO.AUTHENTICATION.APPLICATION.Utils;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using BC = BCrypt.Net.BCrypt;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.Services.ApiKeyAuthentication
{
    public class ApiKeyAuthenticationService : IApiKeyAuthenticationService
    {
        const string _prefix = "BOBERTOAUTH-";
        const int _numberOfSecureBytesToGenerate = 32;
        const int _lengthOfKey = 36;
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
            return EncryptUtils.DecryptString(ApiConfig.ApiKeyAuthenticationConfig.CryptKey, key);
        }
        /// <summary>
        /// Encrypt a key with a Aes secretKey
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string EncryptApiKey(string key)
        {
            return EncryptUtils.EncryptString(ApiConfig.ApiKeyAuthenticationConfig.CryptKey, key);
        }

        /// <summary>
        /// Validate api key with hash and formatted expected
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="CustomException"></exception>
        public bool Validate(string key)
        {
            var isBase64Invalid = EncryptUtils.IsBase64(key) == false;
            if (isBase64Invalid)
            {
                return false;
            }
            var decrypt = DecryptWithPrefix(key);
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

        /* Alteração não mesclada do projeto 'API.BOBERTO.AUTHENTICATION.APPLICATION'
        Antes:
                public GetApiKeyModel Get(string key)
        Após:
                public ApiKeyModel Get(string key)
        */
        public GetApiKeyModel Get(string key)
        {
            var decrypt = DecryptWithPrefix(key);
            if (decrypt == null)
            {
                throw new ApiKeyAuthenticationException(ExceptionTypeEnum.AUTHORIZATION);
            }
            var userId = GetUserId(decrypt);
            var apiKey = DbContext.ApiKey.FirstOrDefault(token => token.UserId.Equals(userId));
            if (apiKey == null)
            {
                throw new ApiKeyAuthenticationException(ExceptionTypeEnum.AUTHORIZATION);
            }
            var apiKeyVerified = BC.Verify(decrypt, apiKey.ApiKey);
            if (apiKeyVerified == false)
            {
                throw new ApiKeyAuthenticationException(ExceptionTypeEnum.AUTHORIZATION);
            }

            /* Alteração não mesclada do projeto 'API.BOBERTO.AUTHENTICATION.APPLICATION'
            Antes:
                        return new GetApiKeyModel()
            Após:
                        return new ApiKeyModel()
            */
            return new GetApiKeyModel()
            {
                ApiKey = decrypt,
                Scopes = apiKey.Scopes.ToArray(),
                UserId = apiKey.UserId
            };
        }

        /// <summary>
        /// Generate API KEY With Scopes foreach user Role
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>GetApiKeyModel</returns>

        /* Alteração não mesclada do projeto 'API.BOBERTO.AUTHENTICATION.APPLICATION'
        Antes:
                public GetApiKeyModel Generate(int userId, RolesEnum role)
        Após:
                public ApiKeyModel Generate(int userId, RolesEnum role)
        */
        public GetApiKeyModel Generate(int userId, RolesEnum role)
        {
            var key = GenerateApiKey();
            var keyWithUserId = CreateUserPrefix(key, userId);
            var hashKey = EncryptApiKey(keyWithUserId);
            var cryptKey = BC.HashPassword(keyWithUserId);
            var scopes = GetScopes(role);

            /* Alteração não mesclada do projeto 'API.BOBERTO.AUTHENTICATION.APPLICATION'
            Antes:
                        return new GetApiKeyModel()
            Após:
                        return new ApiKeyModel()
            */
            return new GetApiKeyModel()
            {
                ApiKey = key,
                ApiKeyHashed = CreateKeyPrefix(hashKey),
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

        /// <summary>
        /// Encrypt with user prefix
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string DecryptWithPrefix(string key)
        {
            key = RemoveKeyPrefix(key);
            return DecryptApiKey(key);
        }
        /// <summary>
        /// Create a user prefix. This is needed because we insert a user id in key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private string CreateUserPrefix(string key, int userId) => string.Concat(userId, "-", key);
        /// <summary>
        /// Create a Key prefix. This is needed to user identifies what api key is it
        /// </summary>
        /// <param name="key"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private string CreateKeyPrefix(string key) => string.Concat(_prefix, key);
        /// <summary>
        /// Remove a prefix from key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string RemoveKeyPrefix(string key)
        {
            return key.Replace(_prefix, "");
        }
        /// <summary>
        /// Get a user id from key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>int userId</returns>
        /// <exception cref="CustomException"></exception>
        private int GetUserId(string key)
        {
            var userId = key.Split("-")[0];
            if (userId == null)
            {
                throw new ApiKeyAuthenticationException(ExceptionTypeEnum.AUTHORIZATION);
            }
            int result;
            int.TryParse(userId, out result);
            return result;
        }
    }
}
