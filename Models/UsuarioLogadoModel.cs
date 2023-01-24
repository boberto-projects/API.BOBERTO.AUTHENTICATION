using api_authentication_boberto.CustomDbContext;

namespace api_authentication_boberto.Models
{
    public class UsuarioLogado
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string NumeroCelular { get; set; }
        public string Nome { get; set; }

        public bool UsarEmail { get; set; }
        public bool UsarNumeroCelular { get; set; }

        public IEnumerable<ApiKeyModel> ApiKeys { get; set; }
    }


}
