using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Models.Response;
using Microsoft.Extensions.Options;
using OtpNet;

namespace api_authentication_boberto.Services.OTP
{
    public class OTPService : IOTPService
    {
        private TwoFactorConfig _twoFactorConfig;
        private byte[] OTPSecret => Base32Encoding.ToBytes(_twoFactorConfig.Key);

        public OTPService(IOptions<TwoFactorConfig> twoFactorConfig)
        {
            _twoFactorConfig = twoFactorConfig.Value;
        }

        public string Generate()
        {
            var size = _twoFactorConfig.Size;
            var totp = new Totp(OTPSecret, totpSize: size, step: _twoFactorConfig.Step);
            var code = totp.ComputeTotp();
            return code;
        }

        public OTPResult Validate(string code)
        {
            var size = _twoFactorConfig.Size;
            var totp = new Totp(OTPSecret, totpSize: size, step: _twoFactorConfig.Step);
            var valid = totp.VerifyTotp(code, out long timeStepMatched);
            return new OTPResult()
            {
                PassoDeTempo = timeStepMatched,
                Valido = valid
            };
        }
    }
}
