namespace API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Response
{
    public class LoginResponse
    {
        public string Tipo { get; set; }
        public string Token { get; set; }
        public bool DuplaAutenticacaoObrigatoria { get; set; }
        public DateTime ExpiraEm { get; set; }
    }
}
