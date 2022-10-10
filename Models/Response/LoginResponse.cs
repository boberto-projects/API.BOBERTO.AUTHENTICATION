namespace api_authentication_boberto.Models.Response
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public bool DuplaAutenticacaoObrigatoria { get; set; }
    }
}
