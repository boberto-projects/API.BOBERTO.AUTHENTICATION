using api_authentication_boberto.Domain.CustomDbContext;

namespace api_authentication_boberto.Services.AuthenticationSecurity
{
    public interface IAuthenticationSecurity
    {
        void CriarCacheUsuario(UsuarioModel user);
        void LimparCacheUsuario();
        bool AtingiuLimiteMaximoDeTentativas();
        void IncrementarTentativa();
        TimeSpan ObterTempoBloqueio();
        TimeSpan ObterTempoBloqueioRestante();
    }
}
