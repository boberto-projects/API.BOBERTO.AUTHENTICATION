using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.OTP.Models;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.Services.OTP
{
    public interface IOTPService
    {
        OTPResult Validate(string code);
        string Generate();
    }
}
