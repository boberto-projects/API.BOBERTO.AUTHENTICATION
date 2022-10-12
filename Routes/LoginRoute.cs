using api_authentication_boberto.CustomDbContext;
using api_authentication_boberto.Interfaces;
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

                var atingiuLimiteMaximoDeTentativas = gerenciadorAutenticacao.AtingiuLimiteMaximoDeTentativas(request.ObterChaveCache);

                if (atingiuLimiteMaximoDeTentativas && contaCadastrada.UsuarioConfig.UsarNumeroCelular == false || atingiuLimiteMaximoDeTentativas && contaCadastrada.UsuarioConfig.UsarEmail == false)
                {
                    return Results.BadRequest("Você errou a senha muitas vezes. Espere um pouco antes de tentar novamente");
                }

                if (atingiuLimiteMaximoDeTentativas)
                {
                    return Results.BadRequest(new LoginResponse()
                    {
                        DuplaAutenticacaoObrigatoria = true
                    });
                }

                var contaExiste = contaCadastrada != null;

                if (contaExiste == false)
                {
                    return Results.Unauthorized();
                }

                var senhaCorreta = BC.Verify(request.Senha, contaCadastrada.Senha);

                if (senhaCorreta == false)
                {
                    gerenciadorAutenticacao.IncrementarTentativa(request.ObterChaveCache);   
                    return Results.Unauthorized();
                }
            
                return Results.Ok(new LoginResponse()
                {
                    DuplaAutenticacaoObrigatoria = atingiuLimiteMaximoDeTentativas,
                    Token = contaCadastrada.GerarTokenJWT(config)
                });
  

            }).WithTags("Autenticação");

          

            app.MapPost("/registrar", [AllowAnonymous] ([FromBody] RegistrarRequest request, [FromServices] DatabaseContext dbContext) =>
            {
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

            }).WithTags("Autenticação");
        }
    }
}
