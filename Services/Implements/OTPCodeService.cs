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
        private byte[] ChaveOTP => Base32Encoding.ToBytes(_twoFactorConfig.Key);

        public OTPCodeService(IOptions<TwoFactorConfig> twoFactorConfig)
        {
            _twoFactorConfig = twoFactorConfig.Value;
        }

        public string GerarCodigoOTP()
        {
            var size = _twoFactorConfig.Size;
            var totp = new Totp(ChaveOTP, totpSize: size, step: _twoFactorConfig.Step);
            var code = totp.ComputeTotp();
            return code;
        }

        public ValidarCodigoOTPResponse ValidarCodigoOTP(string code)
        {
            var size = _twoFactorConfig.Size;
            var totp = new Totp(ChaveOTP, totpSize: size, step: _twoFactorConfig.Step);
            var valid = totp.VerifyTotp(code, out long timeStepMatched);
            return new ValidarCodigoOTPResponse()
            {
                PassoDeTempo = timeStepMatched,
                Valido = valid
            };
        }
    }
}
