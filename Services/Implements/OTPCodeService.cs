using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Models.Response;
using api_authentication_boberto.Services.Interfaces;
using Microsoft.Extensions.Options;
using OtpNet;
using System.Text;

namespace api_authentication_boberto.Services.Implements
{
    public class OTPCodeService : IOTPCode
    {
        private TwoFactorConfig _twoFactorConfig;

        public OTPCodeService(IOptions<TwoFactorConfig> twoFactorConfig)
        {
            _twoFactorConfig = twoFactorConfig.Value;
        }

        public string GerarCodigoOTP()
        {
            var key = Encoding.ASCII.GetBytes(_twoFactorConfig.Key);
            var size = _twoFactorConfig.Size;
            var totp = new Totp(key, totpSize: size, step: _twoFactorConfig.Step);
            var code = totp.ComputeTotp();
            return code;
        }

        public ValidarCodigoOTPResponse ValidarCodigoOTP(string code)
        {
            var key = Encoding.ASCII.GetBytes(_twoFactorConfig.Key);
            var size = _twoFactorConfig.Size;
            var totp = new Totp(key, totpSize: size, step: _twoFactorConfig.Step);
            var valid = totp.VerifyTotp(code, out long timeStepMatched);
            return new ValidarCodigoOTPResponse()
            {
                PassoDeTempo = timeStepMatched,
                Valido = valid
            };
        }
    }
}
