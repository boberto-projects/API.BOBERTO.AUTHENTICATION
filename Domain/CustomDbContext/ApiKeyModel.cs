﻿using System.ComponentModel.DataAnnotations.Schema;

namespace api_authentication_boberto.Domain.CustomDbContext
{
    public class ApiKeyModel
    {
        private List<string> _scopes;
        public int ApiKeyId { get; set; }
        public string ApiKey { get; set; }
        public List<string> Scopes => _scopes;
        public virtual UsuarioModel Usuario { get; set; }
        public int UsuarioId { get; set; }
        public void AddScopes(params string[] scopes) => _scopes = new List<string>(_scopes.Union(scopes));

        public ApiKeyModel()
        {
            _scopes = new List<string>();
        }
    }
}