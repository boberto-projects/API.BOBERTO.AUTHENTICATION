using api_authentication_boberto.Domain.CustomDbContext;
using api_authentication_boberto.Models.Cache;
using api_authentication_boberto.Services.AuthenticationSecurity;
using api_authentication_boberto.Services.RedisService;
using Microsoft.Extensions.Caching.Distributed;

namespace api_authentication_boberto.Services.UserSecurity
{
    public class AuthenticationSecurity : IAuthenticationSecurity
    {
        private IRedisService redisService { get; set; }
        private UserSecurityConfig autenticacaoConfig { get; set; }
        private string CACHE_USUARIO { get; set; }

        public AuthenticationSecurity(IRedisService redisService, IOptions<UserSecurityConfig> gerenciadorAutenticacaoConfig)
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

            redisService.Set(CACHE_USUARIO, obterCacheUsuario);
        }

        public void LimparCacheUsuario()
        {
            redisService.Clear(CACHE_USUARIO);
        }

        public void CriarCacheUsuario(UsuarioModel user)
        {
            CACHE_USUARIO = "TRY_LOGIN_" + user.Email;

            if (redisService.Exists(CACHE_USUARIO))
            {
                return;
            }

            var usuarioCache = new UsuarioCacheModel()
            {
                UltimaTentativa = DateTime.MinValue,
                UltimoLogin = user.UltimoLogin,
                UsuarioId = user.UsuarioId,
                AcessoBloqueado = false,
                Email = user.Email,
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

        public TimeSpan ObterTempoBloqueio()
        {
            return TimeSpan.FromSeconds(autenticacaoConfig.SegundosExpiracao);
        }

        private UsuarioCacheModel ObterCacheUsuario()
        {
            return redisService.Get<UsuarioCacheModel>(CACHE_USUARIO);
        }
    }
}
