using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Models;
using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.CurrentUser.Models;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.Services.CurrentUser
{
    public interface ICurrentUserService
    {
        /// <summary>
        /// Get current user profile
        /// </summary>
        public Profile GetCurrentProfile();

        /// <summary>
        /// get current user pair authentication
        /// </summary>
        public PairAuthentication GetPairAuthenticationEnabled();

        /// <summary>
        /// Enable user authentication
        /// </summary>
        public void EnablePairAuthentication(PairAuthentication pairAuthentication);

    }
}
