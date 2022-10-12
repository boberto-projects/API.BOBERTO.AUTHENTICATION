using api_authentication_boberto.CustomDbContext;
using api_authentication_boberto.Interfaces;
using api_authentication_boberto.Models;
using api_authentication_boberto.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_authentication_boberto.Routes
{
    public static class UsuarioRoute
    {
        public static void AdicionarUsuarioRoute(this WebApplication app)
        {
            app.MapGet("/perfil", [Authorize] ([FromServices] IUsuarioService usuarioLogado) =>
            {
                return usuarioLogado.ObterUsuarioLogado();
            }).WithTags("Usuário");

            app.MapPost("/ativarDuplaAutenticacao", [Authorize] ([
            FromBody] AtivarDuplaAutenticacaoRequest request,
            [FromServices] DatabaseContext dbContext, [FromServices] IUsuarioService usuarioLogado) =>
            {
                usuarioLogado.AtivarAutenticacaoDupla(new AutenticacaoDupla()
                {
                    UsarEmail = request.UsarEmail,
                    UsarNumeroCelular = request.UsarNumeroCelular
                });

                return Results.Ok();
            }).WithTags("Usuário");
        }
    }
}
