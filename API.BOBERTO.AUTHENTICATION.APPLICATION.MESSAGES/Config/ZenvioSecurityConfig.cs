namespace API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Config
{
    public class ZenvioSecurityConfig
    {
        /// <summary>
        /// Tempo máximo de expiração 
        /// </summary>
        public int SecondsExpiration { get; set; }
        /// <summary>
        /// Quantidade máxima de envio de sms diário
        /// </summary>
        public int MaximumAttempts { get; set; }
    }
}
