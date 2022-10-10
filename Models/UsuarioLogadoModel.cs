namespace api_authentication_boberto.Models
{
    public class UsuarioLogado
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string NumeroCelular { get; set; }
        public string Nome { get; set; }
        public IEnumerable<AutenticacaoDupla> Autenticacoes { get; set; }
    }

    public enum AutenticacaoDupla
    {
        Email,
        NumeroCelular
    }
}
