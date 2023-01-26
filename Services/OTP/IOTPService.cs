using api_authentication_boberto.Models.Response;

namespace api_authentication_boberto.Services.OTP
{
    public interface IOTPService
    {
        OTPResult Validate(string code);
        string Generate();
    }
}
