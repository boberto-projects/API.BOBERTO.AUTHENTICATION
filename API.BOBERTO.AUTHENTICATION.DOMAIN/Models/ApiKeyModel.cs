namespace API.BOBERTO.AUTHENTICATION.DOMAIN.Models
{
    public class ApiKeyModel
    {
        private List<string> _scopes;
        public int ApiKeyId { get; set; }
        public string ApiKey { get; set; }
        public List<string> Scopes => _scopes;
        public virtual UserModel User { get; set; }
        public int UserId { get; set; }
        public void AddScopes(params string[] scopes) => _scopes = new List<string>(_scopes.Union(scopes));

        public ApiKeyModel()
        {
            _scopes = new List<string>();
        }
    }
}
