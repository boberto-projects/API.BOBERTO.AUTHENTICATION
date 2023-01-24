namespace api_authentication_boberto.CustomDbContext
{
    public class ApiKeyModel
    {
        public int ApiKeyId { get; set; }
        public string ApiKey { get; set; }
        public IEnumerable<string> Scopes { get; set; }
        public virtual UsuarioModel Usuario { get; set; }
        public int UsuarioId { get; set; }

    }
}
