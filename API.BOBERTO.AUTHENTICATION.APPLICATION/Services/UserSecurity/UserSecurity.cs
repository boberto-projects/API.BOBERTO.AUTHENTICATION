using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.Redis;
using api_authentication_boberto.Domain.CustomDbContext;
using api_authentication_boberto.Models.Cache;
using api_authentication_boberto.Models.Config;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.Services.UserSecurity
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
            var getUserCache = GetUserCache();
            var loginAttempts = getUserCache.LoginAttempts;
            if (loginAttempts >= autenticationConfig.MaximumAttempts)
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
            var getUserCache = GetUserCache();
            getUserCache.LoginAttempts += 1;
            getUserCache.LastAttempt = DateTime.Now;
            redisService.Set(USER_CACHE, getUserCache);
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

            var usuarioCache = new UserCacheModel()
            {
                LastAttempt = DateTime.MinValue,
                LastLogin = user.LastLogin,
                UserId = user.UserId,
                Blocked = false,
                Email = user.Email,
                LoginAttempts = 0,
            };
            var cacheOptions = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(autenticationConfig.SecondsExpiration)
            };

            redisService.Set(USER_CACHE, usuarioCache, cacheOptions);
        }

        public TimeSpan GetBlockTimeRemaining()
        {
            var currentDate = DateTime.Now;
            var lastLogin = GetUserCache().LastLogin;
            var timeLeft = lastLogin.Add(GetBlockTime());
            var timeBlock = currentDate.Subtract(timeLeft);
            return timeBlock;
        }

        public TimeSpan GetBlockTime()
        {
            return TimeSpan.FromSeconds(autenticationConfig.SecondsExpiration);
        }

        private UserCacheModel GetUserCache()
        {
            return redisService.Get<UserCacheModel>(USER_CACHE);
        }
    }
}
