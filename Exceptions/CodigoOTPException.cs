using api_authentication_boberto.Models.Enums;
using api_authentication_boberto.Models.Response;

namespace api_authentication_boberto.Exceptions
{
    [Serializable]
    public class CodigoOTPException : Exception
    {
        public OTPEnum Tipo { get; }
        public int CodigoDeStatus { get; }

        public CodigoOTPException(OTPEnum codigoStatus, string message)
       : base(message)
        {
            Tipo = codigoStatus;
            CodigoDeStatus = 401;
        }

        public CodigoOTPExceptionResponse ObterResponse()
        {
            var response = new CodigoOTPExceptionResponse()
            {
                Tipo = ObterTipo(Tipo),
                Mensagem = Message
            };

            return response;
        }

        private string ObterTipo(OTPEnum codigoStatus)
        {
            switch (codigoStatus)
            {
                case OTPEnum.OTPInvalid:
                    return "codigo_otp_invalido";
                case OTPEnum.OTPNOTINFORMED:
                    return "codigo_otp_nao_informado";
            }

            return "nao_reconhecido";
        }

    }
}
