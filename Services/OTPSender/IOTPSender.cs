namespace api_authentication_boberto.Services.SenderService
{
    /// <summary>
    /// Só deve ser usada pelo aplicativo.
    /// </summary>
    public interface IOTPSender
    {
        void EnviarCodigoSMS(string numeroCelular, string codigo);

        void EnviarCodigoEmail(string email, string codigo);
    }
}
