using api_authentication_boberto.Models.Response;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.Services.OTP
{
    public interface IOTPService
    {
        OTPResult Validate(string code);
        string Generate();
    }
}
