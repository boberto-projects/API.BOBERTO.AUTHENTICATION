namespace API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Models
{
    public class PairAuthentication
    {
        public bool UsarEmail { get; set; }
        public bool UsarNumeroCelular { get; set; }
        public string NumeroCelular { get; set; }
        public string Email { get; set; }
    }
}
