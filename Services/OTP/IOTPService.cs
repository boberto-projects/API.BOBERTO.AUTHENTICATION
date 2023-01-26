namespace api_authentication_boberto.Services.OTP
{
    public interface IOTPService
    {
        ValidarCodigoOTPResponse ValidarCodigoOTP(string code);

        string GerarCodigoOTP();
    }
}
