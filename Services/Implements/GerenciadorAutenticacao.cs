using api_authentication_boberto.CustomDbContext;
using api_authentication_boberto.Interfaces;
using api_authentication_boberto.Models;
using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;

namespace api_authentication_boberto.Services.Implements
{
    public class GerenciadorAutenticacao
    {   
        private IRedisService redisService { get; set; }
        private GerenciadorAutenticacaoConfig autenticacaoConfig { get; set; }
        private string CACHE_USUARIO { get; set; }

        public GerenciadorAutenticacao(IRedisService redisService, IOptions<GerenciadorAutenticacaoConfig> gerenciadorAutenticacaoConfig)
        {
            this.redisService = redisService;
            autenticacaoConfig = gerenciadorAutenticacaoConfig.Value;
        }
        public bool AtingiuLimiteMaximoDeTentativas()
        {
            var obterCacheUsuario = ObterCacheUsuario();
            var tentativasDeLogin = obterCacheUsuario.TentativasDeLogin;

            if (tentativasDeLogin >= autenticacaoConfig.QuantidadeMaximaTentativas)
            {
                return true;
            }
            return false;
        }

        public void IncrementarTentativa()
        {
            if (AtingiuLimiteMaximoDeTentativas())
            {
                return;
            }
            var obterCacheUsuario = ObterCacheUsuario();

            obterCacheUsuario.TentativasDeLogin += 1;
            obterCacheUsuario.UltimaTentativa = DateTime.Now;
            obterCacheUsuario.AcessoBloqueado = true;

            redisService.Set(CACHE_USUARIO, obterCacheUsuario);
        }

        public void LimparCacheUsuario()
        {
            redisService.Clear(CACHE_USUARIO);
        }

        public void CriarCacheUsuario(UsuarioModel usuario)
        {
            CACHE_USUARIO = "TRY_LOGIN_" + usuario.Email;

            if (redisService.Exists(CACHE_USUARIO))
            {
                return;
            }

            var usuarioCache = new UsuarioCacheModel()
            {
                UltimaTentativa = DateTime.MinValue,
                UltimoLogin = usuario.UltimoLogin,
                UsuarioId = usuario.UsuarioId,
                AcessoBloqueado = false,
                Email = usuario.Email,
                TentativasDeLogin = 0,
            };
            var cacheOptions = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(autenticacaoConfig.SegundosExpiracao)
            };

            redisService.Set(CACHE_USUARIO, usuarioCache, cacheOptions);
        }

        public TimeSpan ObterTempoBloqueioRestante()
        {
            var dataAtual = DateTime.Now;
            var ultimoLogin = ObterCacheUsuario().UltimoLogin;
            var tempoRestante = ultimoLogin.Add(ObterTempoBloqueio());
            var tempoBloqueio = dataAtual.Subtract(tempoRestante);
            return tempoBloqueio;
            ///tempo bloqueio = data atual - (ultimo login + tempo bloqueio) 
        }

        private TimeSpan ObterTempoBloqueio()
        {
            return TimeSpan.FromSeconds(autenticacaoConfig.SegundosExpiracao);
        }

        private UsuarioCacheModel ObterCacheUsuario()
        {
            return redisService.Get<UsuarioCacheModel>(CACHE_USUARIO);
        }
    }
}
