namespace api_authentication_boberto.Models.Request
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Senha { get; set; }

        public string ObterChaveCache => "TRY_LOGIN_" + this.Email;
 
    }
}
