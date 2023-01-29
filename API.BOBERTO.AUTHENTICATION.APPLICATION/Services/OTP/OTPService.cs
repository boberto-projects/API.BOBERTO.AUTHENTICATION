using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Config;
using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.OTP.Models;
using Microsoft.Extensions.Options;
using OtpNet;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.Services.OTP
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
                TimeStep = timeStepMatched,
                Valid = valid
            };
        }
    }
}
