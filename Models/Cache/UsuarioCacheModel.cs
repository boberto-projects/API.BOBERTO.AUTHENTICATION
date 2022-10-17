namespace api_authentication_boberto.Models.Cache
{
    public class UsuarioCacheModel
    {
        public DateTime? UltimaTentativa { get; set; }
        public DateTime UltimoLogin { get; set; }
        public int TentativasDeLogin { get; set; }
        public string? Email { get; set; }
        public int? UsuarioId { get; set; }
        public bool AcessoBloqueado { get; set; }

        public UsuarioCacheModel()
        {

        }
    }
}
