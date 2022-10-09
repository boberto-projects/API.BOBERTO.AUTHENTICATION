namespace api_authentication_boberto.Models.Request
{
    public class RegistrarRequest
    {
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Nome { get; set; }
        public string NumeroCelular { get; set; }
    }
}
