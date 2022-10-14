using api_authentication_boberto.Interfaces;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using api_authentication_boberto.Models;
using api_authentication_boberto.CustomDbContext;
using RestEase.Implementation;

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

        public void AtivarAutenticacaoDupla(AutenticacaoDupla autenticacoes)
        {
            var idUsuario = ObterUsuarioLogado().Id;
            var usuario = _dbContext.Usuarios.FirstOrDefault(x => x.UsuarioId.Equals(idUsuario));

            var usarEmail = autenticacoes.UsarEmail;
            var usarNumeroCelular = autenticacoes.UsarNumeroCelular;

            if (usarEmail)
            {
                usuario.Email = autenticacoes.Email;
                usuario.UsuarioConfig.UsarEmail = usarEmail;
            }
            if (usarNumeroCelular)
            {
                usuario.NumeroCelular = autenticacoes.NumeroCelular;
                usuario.UsuarioConfig.UsarNumeroCelular = usarNumeroCelular;
            }
     
            _dbContext.SaveChanges();
        }

        public AutenticacaoDupla ObterAutenticacaoDuplaAtiva()
        {
            var usuarioLogado = ObterUsuarioLogado();
            return new AutenticacaoDupla()
            {
                UsarEmail = usuarioLogado.UsarEmail,
                UsarNumeroCelular = usuarioLogado.UsarNumeroCelular,
                Email = usuarioLogado.Email,
                NumeroCelular = usuarioLogado.NumeroCelular
            };
        }
    }
}
