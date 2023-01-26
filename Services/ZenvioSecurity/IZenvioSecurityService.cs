using api_authentication_boberto.Models.Cache;

namespace api_authentication_boberto.Services.ZenvioSecurity
{
    public interface IZenvioSecurityService
    {
        void CreateCache();
        bool ReachedMaximumLimitOfAttempts();
        void IncrementAttemp();
        TimeSpan GetBlockTime();
        TimeSpan GetBlockTimeRemaining();
        ZenvioCacheModel GetCahe();
    }
}
