namespace api_authentication_boberto.Models.Request
{
    public class TwoFactorVerifyRequest
    {
        public string Email { get; set; }
        public string Code { get; set; }

        public string ObterChaveCache => "TRY_LOGIN_" + this.Email;
    }
}
