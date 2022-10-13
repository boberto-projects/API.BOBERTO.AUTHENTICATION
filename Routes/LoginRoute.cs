using api_authentication_boberto.CustomDbContext;
using api_authentication_boberto.Exceptions;
using api_authentication_boberto.Interfaces;
using api_authentication_boberto.Models;
using api_authentication_boberto.Models.Request;
using api_authentication_boberto.Models.Response;
using api_authentication_boberto.Services.Implements;
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
            app.MapPost("/autenticar", [AllowAnonymous] ([FromBody] LoginRequest request, [FromServices] IRedisService redisService,
                [FromServices] DatabaseContext dbContext,
                [FromServices] GerenciadorAutenticacao gerenciadorAutenticacao,
                [FromServices] IConfiguration config) =>
            {

                var contaCadastrada = dbContext.Usuarios.Include(c => c.UsuarioConfig).FirstOrDefault(e => e.Email.Equals(request.Email));

                var contaExiste = contaCadastrada != null;

                if (contaExiste == false)
                {
                    throw new CustomException(StatusCodeEnum.NaoAutorizado, "Conta não existe.");
                }

                var atingiuLimiteMaximoDeTentativas = gerenciadorAutenticacao.AtingiuLimiteMaximoDeTentativas(request.ObterChaveCache);

                if (atingiuLimiteMaximoDeTentativas && contaCadastrada.UsuarioConfig.UsarNumeroCelular == false || atingiuLimiteMaximoDeTentativas && contaCadastrada.UsuarioConfig.UsarEmail == false)
                {
                    throw new CustomException(StatusCodeEnum.NaoAutorizado, "Você errou a senha muitas vezes. Espere um pouco antes de tentar novamente.");
                }

                if (atingiuLimiteMaximoDeTentativas)
                {
                    return Results.BadRequest(new LoginResponse()
                    {
                        DuplaAutenticacaoObrigatoria = true
                    });
                }
 
                var senhaCorreta = BC.Verify(request.Senha, contaCadastrada.Senha);

                if (senhaCorreta == false)
                {
                    gerenciadorAutenticacao.IncrementarTentativa(request.ObterChaveCache);
                    throw new CustomException(StatusCodeEnum.NaoAutorizado, "Dados inválidos.");
                }

                return Results.Ok(new LoginResponse()
                {
                    DuplaAutenticacaoObrigatoria = atingiuLimiteMaximoDeTentativas,
                    Token = contaCadastrada.GerarTokenJWT(config)
                });
  

            }).WithTags("Autenticação");

          
            ///separar essa rota em outro lugar depois.
            app.MapPost("/registrar", [AllowAnonymous] ([FromBody] RegistrarRequest request, [FromServices] DatabaseContext dbContext) =>
            {
                if(string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Senha))
                {
                    throw new CustomException(StatusCodeEnum.Negocio, "Email e senha são obrigatórios");
                }

                var emailEmUso = dbContext.Usuarios.Count(x => x.Email.Equals(request.Email)) > 0;

                if(emailEmUso)
                {
                    throw new CustomException(StatusCodeEnum.Negocio, "Email já em uso.");
                }

                string hashed = BC.HashPassword(request.Senha);

                var usuarioConfig = new UsuarioConfigModel()
                {
                    UsarEmail = true
                };

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
