using API.BOBERTO.AUTHENTICATION.DOMAIN.Models;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.Services.UserSecurity
{
    public interface IUserSecurity
    {
        void CreateUserCache(UserModel user);
        void ClearUserCache();
        bool ReachedMaximumLimitOfAttempts();
        void IncrementAttemp();
        TimeSpan GetBlockTime();
        TimeSpan GetBlockTimeRemaining();
    }
}
