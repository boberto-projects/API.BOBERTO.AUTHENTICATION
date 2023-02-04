namespace API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Config
{
    public class UserSecurityConfig
    {
        /// <summary>
        /// Tempo máximo de expiração para as tentativas.
        /// </summary>
        public int SecondsExpiration { get; set; }
        /// <summary>
        /// Quantidade máxima de tentativas possíveis para logar.
        /// </summary>
        public int MaximumAttempts { get; set; }
    }
}
