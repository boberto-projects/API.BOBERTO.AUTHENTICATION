using api_authentication_boberto.Models.Request;
using api_authentication_boberto.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BC = BCrypt.Net.BCrypt;

namespace api_authentication_boberto.Routes
{
    public static class LoginRoute
    {
        public  static void AdicionarLoginRoute(this WebApplication app)
        {
            app.MapPost("/autenticar", [AllowAnonymous] ([FromBody] LoginRequest request, [FromServices] DatabaseContext dbContext, [FromServices] IConfiguration config) =>
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
                    return Results.Unauthorized();
                }

                return Results.Ok(new LoginResponse()
                {
                    Token = contaCadastrada.GerarTokenJWT(config)
                });

            }).WithTags("Autenticação");

            app.MapPost("/registrar", [AllowAnonymous] async ([FromBody] RegistrarRequest request, [FromServices] DatabaseContext dbContext) =>
            {
                string hashed = BC.HashPassword(request.Senha);

                dbContext.Usuarios.Add(new()
                {
                    Email = request.Email,
                    Nome = request.Nome,
                    Senha = hashed,
                    NumeroCelular = request.NumeroCelular

                });
                await dbContext.SaveChangesAsync();

            }).WithTags("Autenticação");
        }
    }
}
