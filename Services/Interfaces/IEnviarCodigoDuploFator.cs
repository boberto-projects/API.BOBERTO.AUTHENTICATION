using api_authentication_boberto.Interfaces;

namespace api_authentication_boberto.Services.Interfaces
{
    /// <summary>
    /// Só deve ser usada em rotas autenticadas.
    /// </summary>
    public interface IEnviarCodigoDuploFator
    {
       void EnviarCodigoSMS(IUsuarioService usuario, string codigo);

       void EnviarCodigoEmail(IUsuarioService usuario, string codigo);
    }
}
