using api_authentication_boberto.Domain.CustomDbContext;
using api_authentication_boberto.Models.Cache;
using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Services.Redis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace api_authentication_boberto.Services.UserSecurity
{
    public class UserSecurity : IUserSecurity
    {
        private IRedisService redisService { get; set; }
        private UserSecurityConfig autenticationConfig { get; set; }
        private string USER_CACHE { get; set; }

        public UserSecurity(IRedisService redisService, IOptions<UserSecurityConfig> gerenciadorAutenticacaoConfig)
        {
            this.redisService = redisService;
            autenticationConfig = gerenciadorAutenticacaoConfig.Value;
        }
        public bool ReachedMaximumLimitOfAttempts()
        {
            var obterCacheUsuario = GetUserCache();
            var tentativasDeLogin = obterCacheUsuario.TentativasDeLogin;

            if (tentativasDeLogin >= autenticationConfig.MaximumAttempts)
            {
                return true;
            }
            return false;
        }

        public void IncrementAttemp()
        {
            if (ReachedMaximumLimitOfAttempts())
            {
                return;
            }
            var obterCacheUsuario = GetUserCache();

            obterCacheUsuario.TentativasDeLogin += 1;
            obterCacheUsuario.UltimaTentativa = DateTime.Now;

            redisService.Set(USER_CACHE, obterCacheUsuario);
        }

        public void ClearUserCache()
        {
            redisService.Clear(USER_CACHE);
        }

        public void CreateUserCache(UserModel user)
        {
            USER_CACHE = "TRY_LOGIN_" + user.Email;

            if (redisService.Exists(USER_CACHE))
            {
                return;
            }

            var usuarioCache = new UsuarioCacheModel()
            {
                UltimaTentativa = DateTime.MinValue,
                UltimoLogin = user.LastLogin,
                UsuarioId = user.UserId,
                AcessoBloqueado = false,
                Email = user.Email,
                TentativasDeLogin = 0,
            };
            var cacheOptions = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(autenticationConfig.SecondsExpiration)
            };

            redisService.Set(USER_CACHE, usuarioCache, cacheOptions);
        }

        public TimeSpan GetBlockTimeRemaining()
        {
            var dataAtual = DateTime.Now;
            var ultimoLogin = GetUserCache().UltimoLogin;
            var tempoRestante = ultimoLogin.Add(GetBlockTime());
            var tempoBloqueio = dataAtual.Subtract(tempoRestante);
            return tempoBloqueio;
            ///tempo bloqueio = data atual - (ultimo login + tempo bloqueio) 
        }

        public TimeSpan GetBlockTime()
        {
            return TimeSpan.FromSeconds(autenticationConfig.SecondsExpiration);
        }

        private UsuarioCacheModel GetUserCache()
        {
            return redisService.Get<UsuarioCacheModel>(USER_CACHE);
        }
    }
}
