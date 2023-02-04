

using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions;
using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions.Models;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Request
{
    public class SendOTPSMSRequest
    {
        public string NumeroCelular { get; set; }

        public void Validar()
        {
            if (string.IsNullOrEmpty(NumeroCelular))
            {
                throw new CustomException(StatusCodeEnum.BUSINESS, "PhoneNumber não informado.");
            }
        }
    }
}
