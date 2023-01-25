using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_authentication_boberto.Domain.CustomDbContext
{
    public class UsuarioConfigModel
    {
        public int UsuarioConfigId { get; set; }
        public bool UsarEmail { get; set; }
        public bool UsarNumeroCelular { get; set; }

        //public virtual UsuarioModel Usuario { get; set; }
    }
}
