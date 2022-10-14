using api_authentication_boberto.Exceptions;

namespace api_authentication_boberto.Models.Request
{
    public class EnviarCodigoEmailRequest
    {
        public string Email { get; set; }

        public void Validar()
        {
            if(string.IsNullOrEmpty(Email))
            {
                throw new CustomException(StatusCodeEnum.Negocio, "Email não informado");
            }
        }
    }
}
