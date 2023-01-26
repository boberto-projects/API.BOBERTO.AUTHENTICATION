using api_authentication_boberto.Models;
using api_authentication_boberto.Models.Response;

namespace api_authentication_boberto.Exceptions
{
    [Serializable]
    public class CustomException : Exception
    {
        public int CodigoDeStatus { get; }
        public StatusCodeEnum Tipo { get; }

        public CustomException(StatusCodeEnum codigoStatus, string message)
       : base(message)
        {
            Tipo = codigoStatus;
            CodigoDeStatus = (int)codigoStatus;
        }

        public CustomExceptionResponse ObterResponse()
        {
            var response = new CustomExceptionResponse()
            {
                Tipo = ObterTipo(Tipo),
                Mensagem = Message
            };

            return response;
        }

        private string ObterTipo(StatusCodeEnum codigoStatus)
        {
            switch (codigoStatus)
            {
                case StatusCodeEnum.BUSINESS:
                    return "negocio";
                case StatusCodeEnum.NOTAUTHORIZED:
                    return "nao_autorizado";
                case StatusCodeEnum.VALIDATION:
                    return "validacao";

            }

            return "nao_reconhecido";
        }

    }
}
