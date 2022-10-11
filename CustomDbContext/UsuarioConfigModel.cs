using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_authentication_boberto.CustomDbContext
{
    public class UsuarioConfigModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UsuarioConfigId { get; set; }
        public bool UsarEmail { get; set; }
        public bool UsarNumeroCelular { get; set; }
    }
}
