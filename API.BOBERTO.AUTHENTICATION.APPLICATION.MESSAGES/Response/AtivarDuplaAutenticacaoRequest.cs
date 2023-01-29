using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions;
using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions.Models;
namespace API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Response
{
    public class AtivarDuplaAutenticacaoRequest
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Code { get; set; }

        public void Validate()
        {
            var emailInformado = string.IsNullOrEmpty(Email) == false;
            var numeroCelularInformado = string.IsNullOrEmpty(PhoneNumber) == false;

            if (string.IsNullOrEmpty(Code))
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
