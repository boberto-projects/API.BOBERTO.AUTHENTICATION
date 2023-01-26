using api_authentication_boberto.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_authentication_boberto.Domain.CustomDbContext
{
    public class UsuarioModel
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UsuarioId { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string? NumeroCelular { get; set; }
        public DateTime UltimoLogin { get; set; }

        public int UsuarioConfigId { get; set; }
        public virtual UsuarioConfigModel UsuarioConfig { get; set; }
        public ICollection<ApiKeyModel> ApiKeys { get; set; }
        public RolesEnum Role { get; set; }
        public UsuarioModel()
        {
            ApiKeys = new List<ApiKeyModel>();
        }

    }
}
