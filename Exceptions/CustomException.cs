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
                case StatusCodeEnum.Negocio:
                    return "negocio";
                case StatusCodeEnum.NaoAutorizado:
                    return "nao_autorizado";
                case StatusCodeEnum.Validacao:
                    return "validacao";
            }

            return "nao_reconhecido";
        }

    }
}
