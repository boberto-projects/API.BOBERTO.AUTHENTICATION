using api_authentication_boberto.CustomDbContext;
using api_authentication_boberto.Models.Request;
using api_authentication_boberto.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinecraftServer.Api.Services;
using System;
using System.Data.Entity;
using BC = BCrypt.Net.BCrypt;

namespace api_authentication_boberto.Routes
{
    public static class LoginRoute
    {
        public  static void AdicionarLoginRoute(this WebApplication app)
        {
            app.MapPost("/autenticar", [AllowAnonymous] ([FromBody] LoginRequest request, [FromServices] IRedisService redisService ,[FromServices] DatabaseContext dbContext, [FromServices] IConfiguration config) =>
            {
                var contaCadastrada = dbContext.Usuarios.FirstOrDefault(e => e.Email.Equals(request.Email));
                var CHAVE_CACHE = "TRY_LOGIN_" + request.Email;

                var contaExiste = contaCadastrada != null;

                if (contaExiste == false)
                {
                    return Results.Unauthorized();
                }

                var senhaCorreta = BC.Verify(request.Senha, contaCadastrada.Senha);

                if (senhaCorreta == false)
                {
                    IncrementarTentativa();
                    return Results.Unauthorized();
                }

                if (AtingiuMaximoLimiteDeTentativas() && contaCadastrada.UsuarioConfig.UsarNumeroCelular == false || contaCadastrada.UsuarioConfig.UsarEmail == false)
                {
                    return Results.BadRequest("Você errou a senha muitas vezes.");
                }

                redisService.Clear(CHAVE_CACHE);

                return Results.Ok(new LoginResponse()
                {
                    DuplaAutenticacaoObrigatoria = AtingiuMaximoLimiteDeTentativas(),
                    Token = contaCadastrada.GerarTokenJWT(config)
                });
                
                void IncrementarTentativa()
                {
                    var ultimaTentativa = ObterTentativasLogin();
                    redisService.Set(CHAVE_CACHE, ultimaTentativa + 1, 300);
                }
                bool AtingiuMaximoLimiteDeTentativas()
                {
                    var tentativasDeLogin = ObterTentativasLogin();
                    ///colocar em appsettings depois
                    if(tentativasDeLogin >= 3)
                    {
                        return true;
                    }
                    return false;
                }
                int ObterTentativasLogin()
                {
                    return redisService.Get<int>(CHAVE_CACHE);
                }

            }).WithTags("Autenticação");

          

            app.MapPost("/registrar", [AllowAnonymous] ([FromBody] RegistrarRequest request, [FromServices] DatabaseContext dbContext) =>
            {
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

            }).WithTags("Autenticação");
        }
    }
}
