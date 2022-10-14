using api_authentication_boberto.CustomDbContext;
using api_authentication_boberto.Exceptions;
using api_authentication_boberto.Interfaces;
using api_authentication_boberto.Models;
using api_authentication_boberto.Models.Request;
using api_authentication_boberto.Models.Response;
using api_authentication_boberto.Services.Implements;
using api_authentication_boberto.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace api_authentication_boberto.Routes
{
    public static class LoginRoute
    {
        public  static void AdicionarLoginRoute(this WebApplication app)
        {
            app.MapPost("/autenticar", [AllowAnonymous] ([FromBody] LoginRequest request,
                [FromServices] IRedisService redisService,
                [FromServices] DatabaseContext dbContext,
                IOTPCode otpCode,
                [FromServices] GerenciadorAutenticacao gerenciadorAutenticacao,
                [FromServices] IConfiguration config) =>
            {
                request.Validar();

                var contaCadastrada = dbContext.Usuarios.Include(c => c.UsuarioConfig).FirstOrDefault(e => e.Email.Equals(request.Email));

                var contaExiste = contaCadastrada != null;

                if (contaExiste == false)
                {
                    throw new CustomException(StatusCodeEnum.NaoAutorizado, "Conta não existe.");
                }
                ///crio um cache para essa tentativa de login.

                gerenciadorAutenticacao.CriarCacheUsuario(contaCadastrada);

                var atingiuLimiteMaximoDeTentativas = gerenciadorAutenticacao.AtingiuLimiteMaximoDeTentativas();

                ///Pra um usuário com dupla autenticação ativa, ele sempre terá UsarNumeroCelular true e UsarEmail true.
                ///Então eu posso verificar se esse usuário informou um código de OTP pra saber se ele pode logar ou não.
                var duplaAutenticacaoAtiva = contaCadastrada.UsuarioConfig.UsarNumeroCelular  || contaCadastrada.UsuarioConfig.UsarEmail;

                var codigoOtpInformado = string.isnu
                var codigoOtpInformadoValido = string.IsNullOrEmpty(request.Codigo) == false && otpCode.ValidarCodigoOTP(request.Codigo).Valido;

                ///verifico se já tentou logar muitas vezes antes.
                if (atingiuLimiteMaximoDeTentativas && codigoOtpInformadoValido == false)
                {
                    throw new CustomException(StatusCodeEnum.NaoAutorizado, "Você errou a senha muitas vezes. Espere um pouco antes de tentar novamente.");
                }

                ///Se a dupla autenticação estiver ativa e o código não for informado ou inválido.. Eu vou vou obrigar a logar novamente.
                if (duplaAutenticacaoAtiva && codigoOtpInformadoValido == false)
                {
                    throw new CustomException(StatusCodeEnum.NaoAutorizado, "É necessário informar um código OTP para efetuar login.");
                }

                ///Código informado mas não é válido
                if (codigoOtpInformadoValido == false)
                {
                    throw new CustomException(StatusCodeEnum.NaoAutorizado, "Código informado inválido.");
                }

                ///Comparo a senha.
                var senhaCorreta = BC.Verify(request.Senha, contaCadastrada?.Senha);

                if (senhaCorreta == false)
                {
                    gerenciadorAutenticacao.IncrementarTentativa();
                    throw new CustomException(StatusCodeEnum.NaoAutorizado, "Dados inválidos.");
                }
                /// Atualizo que o UltimoLogin do usuário e retorno um sucesso com o JWT.
                contaCadastrada.UltimoLogin = DateTime.Now;
                dbContext.SaveChanges();

                return Results.Ok(new LoginResponse()
                {
                    Token = contaCadastrada.GerarTokenJWT(config)
                });
            }).WithTags("Autenticação");

          
            ///separar essa rota em outro lugar depois.
            app.MapPost("/registrar", [AllowAnonymous] ([FromBody] RegistrarRequest request, [FromServices] DatabaseContext dbContext) =>
            {
                request.Validar();

                var emailEmUso = dbContext.Usuarios.Count(x => x.Email.Equals(request.Email)) > 0;

                if(emailEmUso)
                {
                    throw new CustomException(StatusCodeEnum.Negocio, "Email já em uso.");
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
                    UsuarioConfigId = usuarioConfig.UsuarioConfigId
                });

                dbContext.SaveChanges();

            }).WithTags("Usuario");
        }
    }
}
