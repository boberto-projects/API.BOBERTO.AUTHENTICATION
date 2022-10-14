using api_authentication_boberto.Exceptions;
namespace api_authentication_boberto.Models.Request
{
    public class EnviarCodigoSMSRequest
    {
        public string NumeroCelular { get; set; }

        public void Validar()
        {
            if (string.IsNullOrEmpty(NumeroCelular))
            {
                throw new CustomException(StatusCodeEnum.Negocio, "NumeroCelular não informado.");
            }
        }
    }
}
