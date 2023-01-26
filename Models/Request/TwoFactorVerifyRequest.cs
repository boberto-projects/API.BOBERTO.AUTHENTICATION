using api_authentication_boberto.Exceptions;
using api_authentication_boberto.Models.Enums;

namespace api_authentication_boberto.Models.Request
{
    public class TwoFactorVerifyRequest
    {
        public string Codigo { get; set; }

        public void Validar()
        {
            if (string.IsNullOrEmpty(Codigo))
            {
                throw new CustomException(StatusCodeEnum.BUSINESS, "Código não informado.");
            }
        }
    }
}
