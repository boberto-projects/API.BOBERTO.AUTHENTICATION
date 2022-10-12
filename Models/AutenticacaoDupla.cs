namespace api_authentication_boberto.Models
{
    public class AutenticacaoDupla
    {
        public bool UsarEmail { get; set; }
        public bool UsarNumeroCelular { get; set; }
        public string? NumeroCelular { get; set; }
        public string? Email { get; set; }
    }
}
