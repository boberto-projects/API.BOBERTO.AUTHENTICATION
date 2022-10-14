using api_authentication_boberto.Exceptions;
using Org.BouncyCastle.Asn1.Ocsp;

namespace api_authentication_boberto.Models.Request
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Codigo { get; set; }

        public void Validar()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Senha))
            {
                throw new CustomException(StatusCodeEnum.Negocio, "Email e senha são obrigatórios");
            }
        }
 
    }
}
