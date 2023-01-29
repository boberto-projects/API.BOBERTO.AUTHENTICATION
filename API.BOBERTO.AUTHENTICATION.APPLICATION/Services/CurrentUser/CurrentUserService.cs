using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.CurrentUser.Models;
using api_authentication_boberto.Domain.CustomDbContext;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.Services.CurrentUser
{
    public class CurrentUserService : ICurrentUserService
    {

        private IHttpContextAccessor _httpContextAccessor { get; set; }
        private DatabaseContext _dbContext { get; set; }

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, DatabaseContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }
        public Profile ObterUsuarioLogado()
        {
            int.TryParse(_httpContextAccessor.HttpContext.User.FindFirstValue("UserId"), out int usuarioId);

            var usuario = _dbContext.Usuarios.Include(c => c.UserConfig).FirstOrDefault(e => e.UserId.Equals(usuarioId));

            return new Profile
            {
                Id = usuarioId,
                UsarEmail = usuario.UserConfig.EnabledEmail,
                UsarNumeroCelular = usuario.UserConfig.EnabledPhoneNumber,
                Email = usuario.Email,
                Nome = usuario.Name,
                Role = usuario.Role,
                NumeroCelular = usuario.PhoneNumber
            };
        }

        public void AtivarAutenticacaoDupla(AutenticacaoDupla autenticacoes)
        {
            var idUsuario = ObterUsuarioLogado().Id;
            var usuario = _dbContext.Usuarios.FirstOrDefault(x => x.UserId.Equals(idUsuario));

            var usarEmail = autenticacoes.UsarEmail;
            var usarNumeroCelular = autenticacoes.UsarNumeroCelular;

            if (usarEmail)
            {
                usuario.Email = autenticacoes.Email;
                usuario.UserConfig.EnabledEmail = usarEmail;
            }
            if (usarNumeroCelular)
            {
                usuario.PhoneNumber = autenticacoes.NumeroCelular;
                usuario.UserConfig.EnabledPhoneNumber = usarNumeroCelular;
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
