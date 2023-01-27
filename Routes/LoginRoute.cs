using api_authentication_boberto.Domain.CustomDbContext;
using api_authentication_boberto.Exceptions;
using api_authentication_boberto.Models.Enums;
using api_authentication_boberto.Models.Request;
using api_authentication_boberto.Models.Response;
using api_authentication_boberto.Services.CurrentUser;
using api_authentication_boberto.Services.JWT;
using api_authentication_boberto.Services.OTP;
using api_authentication_boberto.Services.OTPSender;
using api_authentication_boberto.Services.Redis;
using api_authentication_boberto.Services.UserSecurity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace api_authentication_boberto.Routes
{
    public static class LoginRoute
    {
        public static void AddLoginRoute(this WebApplication app)
        {
            app.MapPost("/autenticar", [Authorize(AuthenticationSchemes = "api_key")] ([FromBody] LoginRequest request,
                [FromServices] DatabaseContext dbContext,
                IOTPService otpCode,
                IRedisService redisService,
                IOTPSender enviarCodigoDuploFator,
                IUserSecurity gerenciadorAutenticacao,
                IConfiguration config,
                IJWTService tokenJWTService
                ) =>
            {
                request.Validar();

                var contaCadastrada = dbContext.Usuarios.Include(c => c.UserConfig).FirstOrDefault(e => e.Email.Equals(request.Email));

                var contaExiste = contaCadastrada != null;

                if (contaExiste == false)
                {
                    throw new CustomException(StatusCodeEnum.NOTAUTHORIZED, "Conta não existe.");
                }
                ///crio um cache para essa tentativa de login.

                gerenciadorAutenticacao.CreateUserCache(contaCadastrada);

                var atingiuLimiteMaximoDeTentativas = gerenciadorAutenticacao.ReachedMaximumLimitOfAttempts();

                ///Pra um usuário com dupla autenticação ativa, ele sempre terá UsarNumeroCelular true e UsarEmail true.
                ///Então eu posso verificar se esse usuário informou um código de OTP pra saber se ele pode logar ou não.
                var duplaAutenticacaoAtiva = contaCadastrada.UserConfig.EnabledPhoneNumber || contaCadastrada.UserConfig.EnabledEmail;

                var codigoOtpExiste = string.IsNullOrEmpty(request.Codigo) == false;

                var codigoOtp = codigoOtpExiste && otpCode.Validate(request.Codigo).Valido;

                ///Se atingiu o limite máximo de tentativas de login falhas e o codigo otp não foi informado
                if (atingiuLimiteMaximoDeTentativas && codigoOtpExiste == false)
                {

                    throw new CustomException(StatusCodeEnum.NOTAUTHORIZED, "Você errou a senha muitas vezes. Espere um pouco antes de tentar novamente.");
                }

                ///Comparo a senha.
                var senhaCorreta = BC.Verify(request.Senha, contaCadastrada?.Password);

                if (senhaCorreta == false)
                {
                    gerenciadorAutenticacao.IncrementAttemp();
                    throw new CustomException(StatusCodeEnum.NOTAUTHORIZED, "Dados inválidos.");
                }

                ///Se a dupla autenticação estiver ativa e o código não for informado ou inválido.. Eu vou retornar um token do tipo token_temporario
                ///um token de 35 segundos que o aplicativo usuará apenas para obter informações
                if (duplaAutenticacaoAtiva && codigoOtpExiste == false)
                {
                    var expiraEm = DateTime.UtcNow.AddSeconds(35);
                    var token_refresh = tokenJWTService.Generate(contaCadastrada, expiraEm);
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
                    throw new CodeOTPException(OTPEnum.OTPInvalid, "Código informado inválido.");
                }
                /// Atualizo que o UltimoLogin do usuário e retorno um sucesso com o JWT.
                contaCadastrada.LastLogin = DateTime.Now;
                var token = tokenJWTService.Generate(contaCadastrada);
                dbContext.SaveChanges();

                return Results.Ok(new LoginResponse()
                {
                    DuplaAutenticacaoObrigatoria = duplaAutenticacaoAtiva,
                    Token = token
                });
            }).WithTags("Autenticação");

            //refresh token
            app.MapPost("/refresh_token", [Authorize] (
                [FromServices] DatabaseContext dbContext,
                [FromBody] RefreshTokenRequest request,
                ICurrentUserService usuarioLogado,
                IJWTService tokenJWTService) =>
            {
                var tokenValido = tokenJWTService.Validate(request.Token);
                if (tokenValido == false)
                {
                    throw new CustomException(StatusCodeEnum.NOTAUTHORIZED, "Token inválido");
                }
                var usuario = usuarioLogado.ObterUsuarioLogado();
                var expiracao = DateTime.UtcNow.AddHours(1);
                var token = tokenJWTService.Generate(new UserModel()
                {
                    UserId = usuario.Id,
                    Email = usuario.Email,
                    Name = usuario.Nome,
                    PhoneNumber = usuario.NumeroCelular
                }, expiracao);

                return Results.Ok(new RefreshTokenResponse()
                {
                    Token = token,
                    ExpiraEm = expiracao
                });
            });


            ///separar essa rota em outro lugar depois.
            app.MapPost("/registrar", [Authorize(AuthenticationSchemes = "api_key")] (
                [FromBody] RegistrarRequest request,
                [FromServices] DatabaseContext dbContext) =>
        {
            request.Validar();

            var emailEmUso = dbContext.Usuarios.Count(x => x.Email.Equals(request.Email)) > 0;

            if (emailEmUso)
            {
                throw new CustomException(StatusCodeEnum.BUSINESS, "Email já em uso.");
            }

            string hashed = BC.HashPassword(request.Senha);

            var usuarioConfig = new UserConfigModel();

            dbContext.UsuariosConfig.Add(usuarioConfig);
            dbContext.SaveChanges();

            dbContext.Usuarios.Add(new()
            {
                Email = request.Email,
                Name = request.Nome,
                Password = hashed,
                PhoneNumber = request.NumeroCelular,
                UserConfigId = usuarioConfig.UserConfigId,
                Role = RolesEnum.USER
            });

            dbContext.SaveChanges();

        }).WithTags("Usuário");
        }
    }
}
