﻿namespace API.BOBERTO.AUTHENTICATION.APPLICATION.Services.OTPSender
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
