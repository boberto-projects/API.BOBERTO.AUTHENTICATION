namespace api_authentication_boberto.Services.OTPSender
{
    /// <summary>
    /// Só deve ser usada pelo aplicativo.
    /// </summary>
    public interface IOTPSender
    {
        void SendSMS(string numeroCelular, string codigo);

        void SendEmail(string email, string codigo);
    }
}
