namespace api_authentication_boberto
{
    public class ApiCicloDeVida
    {
        public DateTime iniciouEm { get; set; }

        public ApiCicloDeVida()
        {
            this.iniciouEm = DateTime.Now;
        }
    }
}
