namespace api_authentication_boberto.Models.Config
{
    public class GerenciadorAutenticacaoConfig
    {
        /// <summary>
        /// Tempo máximo de expiração para as tentativas.
        /// </summary>
        public int SegundosExpiracao { get; set; }  
        /// <summary>
        /// Quantidade máxima de tentativas possíveis para logar.
        /// </summary>
        public int QuantidadeMaximaTentativas { get; set; }

       
    }
}
