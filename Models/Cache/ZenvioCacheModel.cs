namespace api_authentication_boberto.Models.Cache
{
    public class ZenvioCacheModel
    {
        public DateTime? UltimaTentativa { get; set; }
        public int Tentativas { get; set; }
        public bool AcessoBloqueado { get; set; }
    }
}
