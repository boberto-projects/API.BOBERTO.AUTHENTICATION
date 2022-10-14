using api_authentication_boberto.Exceptions;

namespace api_authentication_boberto.Models.Response
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
                throw new CustomException(StatusCodeEnum.Validacao, "Código OTP não informado.");
            }
            if(emailInformado && numeroCelularInformado)
            {
                throw new CustomException(StatusCodeEnum.Validacao, "Necessário informar NumeroCelular ou Email.");
            }
        }
    }
}
