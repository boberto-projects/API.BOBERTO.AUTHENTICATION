namespace api_authentication_boberto.Services.OTPSender
{
    /// <summary>
    /// Só deve ser usada pelo aplicativo.
    /// </summary>
    public interface IOTPSender
    {
        void SendSMS(string phoneNumber, string code);

        void SendEmail(string email, string code);
    }
}
