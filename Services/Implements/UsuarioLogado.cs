using api_authentication_boberto.Interfaces;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using api_authentication_boberto.Models;
using api_authentication_boberto.CustomDbContext;

namespace api_authentication_boberto.Implements
{
    public class UsuarioService : IUsuarioService
    {
      
        private IHttpContextAccessor _httpContextAccessor { get; set; }
        private DatabaseContext _dbContext { get; set; }

        public UsuarioService(IHttpContextAccessor httpContextAccessor, DatabaseContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }
        public UsuarioLogado ObterUsuarioLogado()
        {
            int.TryParse(_httpContextAccessor.HttpContext.User.FindFirstValue("UserId"), out int usuarioId);

            var usuario = _dbContext.Usuarios.Include(c => c.UsuarioConfig).FirstOrDefault(e => e.UsuarioId.Equals(usuarioId));

            return new UsuarioLogado
            {
                Id = usuarioId,
                UsarEmail = usuario.UsuarioConfig.UsarEmail,
                UsarNumeroCelular = usuario.UsuarioConfig.UsarNumeroCelular,
                Email = usuario.Email,
                Nome = usuario.Nome,
                NumeroCelular = usuario.NumeroCelular
            };
        }

        public bool AtivarAutenticacaoDupla()
        {
            throw new NotImplementedException();
        }

        public bool ObterAutenticacaoDuplaAtiva()
        {
            throw new NotImplementedException();
        }
    }
}
