using api_authentication_boberto.Domain.CustomDbContext;

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
