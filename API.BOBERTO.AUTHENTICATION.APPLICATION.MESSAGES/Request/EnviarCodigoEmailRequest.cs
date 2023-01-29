

using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions;
using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions.Models;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Request
{
    public class EnviarCodigoEmailRequest
    {
        public string Email { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(Email))
            {
                throw new CustomException(StatusCodeEnum.BUSINESS, "Email não informado");
            }
        }
    }
}
