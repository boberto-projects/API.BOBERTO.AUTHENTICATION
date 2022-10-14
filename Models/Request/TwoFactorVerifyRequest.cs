using api_authentication_boberto.Exceptions;

namespace api_authentication_boberto.Models.Request
{
    public class TwoFactorVerifyRequest
    {
        public string Codigo { get; set; }

        public void Validar()
        {
            if (string.IsNullOrEmpty(Codigo))
            {
                throw new CustomException(StatusCodeEnum.Negocio, "Código não informado.");
            }
        }
    }
}
