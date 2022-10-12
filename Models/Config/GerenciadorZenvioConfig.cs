namespace api_authentication_boberto.Models.Config
{
    public class GerenciadorZenvioConfig
    {
        /// <summary>
        /// Tempo máximo de expiração 
        /// </summary>
        public int SegundosExpiracao { get; set; }
        /// <summary>
        /// Quantidade máxima de envio de sms diário
        /// </summary>
        public int QuantidadeMaximaTentativas { get; set; }
    }
}
