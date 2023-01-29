

using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions;
using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions.Models;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Request
{
    public class TwoFactorVerifyRequest
    {
        public string Code { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(Code))
            {
                throw new CustomException(StatusCodeEnum.BUSINESS, "Código não informado.");
            }
        }
    }
}
