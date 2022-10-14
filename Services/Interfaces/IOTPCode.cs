using api_authentication_boberto.Models.Response;

namespace api_authentication_boberto.Services.Interfaces
{
    public interface IOTPCode
    {
        ValidarCodigoOTPResponse ValidarCodigoOTP(string code);

        string GerarCodigoOTP();
    }
}
