namespace api_authentication_boberto.Services.Interfaces
{
    public interface IGerenciadorAcesso
    {
        int ObterTentativas(string chave_cache);
        bool AtingiuLimiteMaximoDeTentativas(string chave_cache);
        void IncrementarTentativa(string chave_cache);
        void LimparTentativas(string chave_cache);
        TimeSpan ObterTempoBloqueio();
        TimeSpan ObterTempoBloqueioRestante();
    }
}
