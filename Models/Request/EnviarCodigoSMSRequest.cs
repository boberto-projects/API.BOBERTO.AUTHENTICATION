using api_authentication_boberto.Exceptions;
using api_authentication_boberto.Models.Enums;

namespace api_authentication_boberto.Models.Request
{
    public class EnviarCodigoSMSRequest
    {
        public string NumeroCelular { get; set; }

        public void Validar()
        {
            if (string.IsNullOrEmpty(NumeroCelular))
            {
                throw new CustomException(StatusCodeEnum.BUSINESS, "NumeroCelular não informado.");
            }
        }
    }
}
