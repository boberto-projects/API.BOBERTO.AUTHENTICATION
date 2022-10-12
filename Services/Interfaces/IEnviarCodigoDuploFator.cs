using api_authentication_boberto.Interfaces;

namespace api_authentication_boberto.Services.Interfaces
{
    /// <summary>
    /// Só deve ser usada em rotas autenticadas.
    /// </summary>
    public interface IEnviarCodigoDuploFator
    {
       void EnviarCodigo(IUsuarioService usuario, string codigo);
    }
}
