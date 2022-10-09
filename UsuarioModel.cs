using System.ComponentModel.DataAnnotations;

namespace api_authentication_boberto
{
    public class UsuarioModel
    {
        public int UsuarioId { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}
