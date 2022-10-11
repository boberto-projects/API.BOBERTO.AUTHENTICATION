using api_authentication_boberto.CustomDbContext;
using api_authentication_boberto.Models.Request;
using api_authentication_boberto.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinecraftServer.Api.Services;
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

                return Results.Ok(new LoginResponse()
                {
                    DuplaAutenticacaoObrigatoria = AutenticacaoDuploFatorObrigatoria(),
                    Token = contaCadastrada.GerarTokenJWT(config)
                });

                void IncrementarTentativa()
                {
                    var chave = "TRY_LOGIN_" + request.Email;
                    var ultimaTentativa = redisService.Get<int>(chave);
                    redisService.Set(chave, ultimaTentativa + 1, 300);
                }
                bool AutenticacaoDuploFatorObrigatoria()
                {
                    var chave = "TRY_LOGIN_" + request.Email;
                    var tentativasDeLogin = redisService.Get<int>(chave);
                    if(tentativasDeLogin >= 3)
                    {
                        return true;
                    }
                    return false;
                }

            }).WithTags("Autenticação");

          

            app.MapPost("/registrar", [AllowAnonymous] async ([FromBody] RegistrarRequest request, [FromServices] DatabaseContext dbContext) =>
            {
                string hashed = BC.HashPassword(request.Senha);

                dbContext.Usuarios.Add(new()
                {
                    Email = request.Email,
                    Nome = request.Nome,
                    Senha = hashed,
                    NumeroCelular = request.NumeroCelular,
                    //UsuarioConfig = new UsuarioConfigModel()
                    //{
                    //    UsuarioConfigId = 55,
                    //    UsarEmail = false,
                    //    UsarNumeroCelular = false
                    //}
                });
                await dbContext.SaveChangesAsync();

            }).WithTags("Autenticação");
        }
    }
}
