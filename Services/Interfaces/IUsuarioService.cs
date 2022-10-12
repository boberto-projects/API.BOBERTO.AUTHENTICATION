using api_authentication_boberto.Models;

namespace api_authentication_boberto.Interfaces
{
    public interface IUsuarioService
    {
        /// <summary>
        /// Obter o usuário logado usando o claim principal
        /// </summary>
        public UsuarioLogado ObterUsuarioLogado();

        /// <summary>
        /// Obtém se usuário logado tem autenticação dupla ativa
        /// </summary>
        public bool ObterAutenticacaoDuplaAtiva();

        /// <summary>
        /// Ativa autenticação dupla do usuário
        /// </summary>
        public bool AtivarAutenticacaoDupla();

    }
}
