using api_authentication_boberto.Exceptions;
using api_authentication_boberto.Models.Enums;

namespace API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Response
{
    public class AtivarDuplaAutenticacaoRequest
    {
        public string Email { get; set; }
        public string NumeroCelular { get; set; }
        public string Codigo { get; set; }

        public void Validar()
        {
            var emailInformado = string.IsNullOrEmpty(Email) == false;
            var numeroCelularInformado = string.IsNullOrEmpty(NumeroCelular) == false;

            if (string.IsNullOrEmpty(Codigo))
            {
                throw new CustomException(StatusCodeEnum.VALIDATION, "Código OTP não informado.");
            }
            if (emailInformado && numeroCelularInformado)
            {
                throw new CustomException(StatusCodeEnum.VALIDATION, "Necessário informar PhoneNumber ou Email.");
            }
        }
    }
}
