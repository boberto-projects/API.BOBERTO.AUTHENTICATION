using api_authentication_boberto.Models;
using api_authentication_boberto.Models.Response;

namespace api_authentication_boberto.Exceptions
{
    [Serializable]
    public class CodigoOTPException : Exception
    {
        public CodigoOTPEnum Tipo { get; }
        public int CodigoDeStatus { get; }

        public CodigoOTPException(CodigoOTPEnum codigoStatus, string message)
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

        private string ObterTipo(CodigoOTPEnum codigoStatus)
        {
            switch (codigoStatus)
            {
                case CodigoOTPEnum.CodigoOTPInvalido:
                    return "codigo_otp_invalido";
                case CodigoOTPEnum.CodigoOTPNaoInformado:
                    return "codigo_otp_nao_informado";
            }

            return "nao_reconhecido";
        }

    }
}
