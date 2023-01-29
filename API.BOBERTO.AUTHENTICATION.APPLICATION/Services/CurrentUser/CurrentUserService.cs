using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions;
using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions.Models;
using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Models;
using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.CurrentUser.Models;
using API.BOBERTO.AUTHENTICATION.DOMAIN;
using Microsoft.AspNetCore.Http;
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
        public Profile GetCurrentProfile()
        {
            var currentUserClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if(currentUserClaim == null)
            {
                 throw new CustomException(StatusCodeEnum.NOTAUTHORIZED, "You are not authenticaded.");
            }
            int.TryParse(currentUserClaim.Value, out int userId);
            var user = _dbContext.Usuarios.Include(c => c.UserConfig).FirstOrDefault(e => e.UserId.Equals(userId));
            return new Profile
            {
                Id = userId,
                UsarEmail = user.UserConfig.EnabledEmail,
                UsarNumeroCelular = user.UserConfig.EnabledPhoneNumber,
                Email = user.Email,
                Nome = user.Name,
                Role = user.Role,
                NumeroCelular = user.PhoneNumber
            };
        }

        public void EnablePairAuthentication(PairAuthentication autenticacoes)
        {
            var idUsuario = GetCurrentProfile().Id;
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

        public PairAuthentication GetPairAuthenticationEnabled()
        {
            var usuarioLogado = GetCurrentProfile();
            return new PairAuthentication()
            {
                UsarEmail = usuarioLogado.UsarEmail,
                UsarNumeroCelular = usuarioLogado.UsarNumeroCelular,
                Email = usuarioLogado.Email,
                NumeroCelular = usuarioLogado.NumeroCelular
            };
        }
    }
}
