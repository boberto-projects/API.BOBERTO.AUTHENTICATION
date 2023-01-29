using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Enums.OTP;
using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions.Models.Serialize;
using api_authentication_boberto.Models.Enums;
using api_authentication_boberto.Models.Response;

namespace api_authentication_boberto.Exceptions
{
    [Serializable]
    public class CodeOTPException : Exception
    {
        public OTPEnum Type { get; }
        public int CodigoDeStatus { get; }
        public CodeOTPException(OTPEnum codigoStatus, string message)
       : base(message)
        {
            Type = codigoStatus;
            CodigoDeStatus = 401;
        }

        public CodeOTPExceptionResponse GetResponse()
        {
            var response = new CodeOTPExceptionResponse()
            {
                Type = GetType(),
                Message = Message
            };

            return response;
        }

        private string GetType()
        {
            switch (Type)
            {
                case OTPEnum.OTPInvalid:
                    return "otp_invalid";
                case OTPEnum.OTPNOTINFORMED:
                    return "otp_wrong_format";
            }
            return "otp_wrong_format";
        }

    }
}
