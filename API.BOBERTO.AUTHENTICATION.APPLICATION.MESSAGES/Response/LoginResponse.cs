namespace API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Response
{
    public class LoginResponse
    {
        public string Type { get; set; }
        public string Token { get; set; }
        public bool PairAuthenticationEnabled { get; set; }
        public DateTime ExpireIn { get; set; }
    }
}
