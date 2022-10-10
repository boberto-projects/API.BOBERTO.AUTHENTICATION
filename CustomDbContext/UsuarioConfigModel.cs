using System.ComponentModel.DataAnnotations.Schema;

namespace api_authentication_boberto.CustomDbContext
{
    public class UsuarioConfigModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UsuarioConfigId { get; set; }

        public int UsuarioId { get; set; }
        public bool UsarEmail { get; set; }
        public bool UsarNumeroCelular { get; set; }
        public virtual UsuarioModel Usuario { get; set; }
    }
}
