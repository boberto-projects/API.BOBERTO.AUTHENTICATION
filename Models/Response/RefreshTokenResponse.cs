namespace api_authentication_boberto.Models.Response
{
    public class RefreshTokenResponse
    {
        public string Token { get; set; }
        public DateTime ExpiraEm { get; set; }
    }
}
