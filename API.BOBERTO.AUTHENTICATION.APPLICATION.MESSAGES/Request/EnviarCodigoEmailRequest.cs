using api_authentication_boberto.Exceptions;
using api_authentication_boberto.Models.Enums;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Request
{
    public class EnviarCodigoEmailRequest
    {
        public string Email { get; set; }

        public void Validar()
        {
            if (string.IsNullOrEmpty(Email))
            {
                throw new CustomException(StatusCodeEnum.BUSINESS, "Email não informado");
            }
        }
    }
}
