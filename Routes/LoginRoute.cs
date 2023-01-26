using api_authentication_boberto.Domain.CustomDbContext;
using api_authentication_boberto.Exceptions;
using api_authentication_boberto.Models.Enums;
using api_authentication_boberto.Models.Request;
using api_authentication_boberto.Models.Response;
using api_authentication_boberto.Services.Implements;
using api_authentication_boberto.Services.OTPSender;
using api_authentication_boberto.Services.Redis;
using api_authentication_boberto.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using BC = BCrypt.Net.BCrypt;

namespace api_authentication_boberto.Routes
{
    public static class LoginRoute
    {
        public static void AdicionarLoginRoute(this WebApplication app)
        {
            app.MapPost("/autenticar", [AllowAnonymous] ([FromBody] LoginRequest request,
                [FromServices] IRedisService redisService,
                [FromServices] DatabaseContext dbContext,
                IOTPService otpCode,
                IOTPSender enviarCodigoDuploFator,
                [FromServices] UserSecurity gerenciadorAutenticacao,
                [FromServices] IConfiguration config,
                [FromServices] JWTService tokenJWTService
                ) =>
            {
                request.Validar();

                var contaCadastrada = dbContext.Usuarios.Include(c => c.UsuarioConfig).FirstOrDefault(e => e.Email.Equals(request.Email));

                var contaExiste = contaCadastrada != null;

                if (contaExiste == false)
                {
                    throw new CustomException(StatusCodeEnum.NOTAUTHORIZED, "Conta não existe.");
                }
                ///crio um cache para essa tentativa de login.

                gerenciadorAutenticacao.CriarCacheUsuario(contaCadastrada);

                var atingiuLimiteMaximoDeTentativas = gerenciadorAutenticacao.AtingiuLimiteMaximoDeTentativas();

                ///Pra um usuário com dupla autenticação ativa, ele sempre terá UsarNumeroCelular true e UsarEmail true.
                ///Então eu posso verificar se esse usuário informou um código de OTP pra saber se ele pode logar ou não.
                var duplaAutenticacaoAtiva = contaCadastrada.UsuarioConfig.UsarNumeroCelular || contaCadastrada.UsuarioConfig.UsarEmail;

                var codigoOtpExiste = string.IsNullOrEmpty(request.Codigo) == false;

                var codigoOtp = codigoOtpExiste && otpCode.ValidarCodigoOTP(request.Codigo).Valido;

                ///Se atingiu o limite máximo de tentativas de login falhas e o codigo otp não foi informado
                if (atingiuLimiteMaximoDeTentativas && codigoOtpExiste == false)
                {

                    throw new CustomException(StatusCodeEnum.NOTAUTHORIZED, "Você errou a senha muitas vezes. Espere um pouco antes de tentar novamente.");
                }

                ///Comparo a senha.
                var senhaCorreta = BC.Verify(request.Senha, contaCadastrada?.Senha);

                if (senhaCorreta == false)
                {
                    gerenciadorAutenticacao.IncrementarTentativa();
                    throw new CustomException(StatusCodeEnum.NOTAUTHORIZED, "Dados inválidos.");
                }

                ///Se a dupla autenticação estiver ativa e o código não for informado ou inválido.. Eu vou retornar um token do tipo token_temporario
                ///um token de 35 segundos que o aplicativo usuará apenas para obter informações
                if (duplaAutenticacaoAtiva && codigoOtpExiste == false)
                {
                    var expiraEm = DateTime.UtcNow.AddSeconds(35);
                    var token_refresh = tokenJWTService.GerarTokenJWT(contaCadastrada, expiraEm);
                    return Results.Ok(new LoginResponse()
                    {
                        Tipo = "token_temporario",
                        Token = token_refresh,
                        DuplaAutenticacaoObrigatoria = duplaAutenticacaoAtiva,
                        ExpiraEm = expiraEm
                    });
                }
                ///Código informado mas não é válido
                if (codigoOtpExiste && codigoOtp == false)
                {
                    throw new CodigoOTPException(OTPEnum.OTPInvalid, "Código informado inválido.");
                }
                /// Atualizo que o UltimoLogin do usuário e retorno um sucesso com o JWT.
                contaCadastrada.UltimoLogin = DateTime.Now;
                var token = tokenJWTService.GerarTokenJWT(contaCadastrada);
                dbContext.SaveChanges();

                return Results.Ok(new LoginResponse()
                {
                    DuplaAutenticacaoObrigatoria = duplaAutenticacaoAtiva,
                    Token = token
                });
            }).WithTags("Autenticação");

            //refresh token
            app.MapPost("/refresh_token", [Authorize] ([FromBody] RefreshTokenRequest request, ICurrentUserService usuarioLogado, [FromServices] JWTService tokenJWTService, [FromServices] DatabaseContext dbContext) =>
            {
                var tokenValido = tokenJWTService.ValidarTokenJWT(request.Token);
                if (tokenValido == false)
                {
                    throw new CustomException(StatusCodeEnum.NOTAUTHORIZED, "Token inválido");
                }
                var usuario = usuarioLogado.ObterUsuarioLogado();
                var expiracao = DateTime.UtcNow.AddHours(1);
                var token = tokenJWTService.GerarTokenJWT(new UsuarioModel()
                {
                    UsuarioId = usuario.Id,
                    Email = usuario.Email,
                    Nome = usuario.Nome,
                    NumeroCelular = usuario.NumeroCelular
                }, expiracao);

                return Results.Ok(new RefreshTokenResponse()
                {
                    Token = token,
                    ExpiraEm = expiracao
                });
            });


            ///separar essa rota em outro lugar depois.
            app.MapPost("/registrar", [AllowAnonymous] ([FromBody] RegistrarRequest request, [FromServices] DatabaseContext dbContext) =>
        {
            request.Validar();

            var emailEmUso = dbContext.Usuarios.Count(x => x.Email.Equals(request.Email)) > 0;

            if (emailEmUso)
            {
                throw new CustomException(StatusCodeEnum.BUSINESS, "Email já em uso.");
            }

            string hashed = BC.HashPassword(request.Senha);

            var usuarioConfig = new UsuarioConfigModel();

            dbContext.UsuariosConfig.Add(usuarioConfig);
            dbContext.SaveChanges();

            dbContext.Usuarios.Add(new()
            {
                Email = request.Email,
                Nome = request.Nome,
                Senha = hashed,
                NumeroCelular = request.NumeroCelular,
                UsuarioConfigId = usuarioConfig.UsuarioConfigId,
                Role = RolesEnum.USER
            });

            dbContext.SaveChanges();

        }).WithTags("Usuário");
        }
    }
}
