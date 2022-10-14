
namespace api_authentication_boberto.Services.Interfaces
{
    /// <summary>
    /// Só deve ser usada pelo aplicativo.
    /// </summary>
    public interface IEnviarCodigoDuploFator
    {
       void EnviarCodigoSMS(string numeroCelular, string codigo);

       void EnviarCodigoEmail(string email, string codigo);
    }
}
