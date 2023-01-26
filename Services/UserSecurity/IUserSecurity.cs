using api_authentication_boberto.Domain.CustomDbContext;

namespace api_authentication_boberto.Services.UserSecurity
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
