using api_authentication_boberto.CustomDbContext;
using api_authentication_boberto.Interfaces;
using api_authentication_boberto.Models;
using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Models.Request;
using api_authentication_boberto.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OtpNet;
using System.Text;

namespace api_authentication_boberto.Routes
{
    public static class OtpRoute
    {
        public static void AdicionarOtpRoute(this WebApplication app)
        {
            app.MapPost("/gerarotp", [Authorize] async ([FromServices] DatabaseContext dbContext,[FromServices] IOptions<DiscordAPIConfig> discordApiConfig,
                IOptions<TwoFactorConfig> twoFactorConfig, [FromServices] IUsuarioService usuarioLogado) =>
            {
                var key = Encoding.ASCII.GetBytes(twoFactorConfig.Value.Key);
                var size = twoFactorConfig.Value.Size;

                var totp = new Totp(key, totpSize: size);

                var usuarioAtual = usuarioLogado.ObterUsuarioLogado();

                var code = totp.ComputeTotp();

                return Results.Ok(new GenerateOtpResponse()
                {
                    Code = code
                });

            }).WithTags("Autenticação");

            //USANDO Time-based OTPs
            app.MapPost("/validarotp", [AllowAnonymous] async ([FromBody] TwoFactorVerifyRequest request, [FromServices] IOptions<TwoFactorConfig> twoFactorConfig) =>
            {
                var key = Encoding.ASCII.GetBytes(twoFactorConfig.Value.Key);
                var size = twoFactorConfig.Value.Size;

                var totp = new Totp(key, totpSize: size);

                var valid = totp.VerifyTotp(request.Code, out long timeStepMatched);

                return Results.Ok(valid);

            }).WithTags("Autenticação");

            app.MapPost("/ativarDuplaAutenticacao", [Authorize] async ([FromBody] AtivarDuplaAutenticacaoRequest request,
                [FromServices] IOptions<TwoFactorConfig> twoFactorConfig, 
                [FromServices] DatabaseContext dbContext, [FromServices] IUsuarioService usuarioLogado) =>
            {
                var idUsuario = usuarioLogado.ObterUsuarioLogado().Id;
                var usuario = dbContext.Usuarios.FirstOrDefault(x => x.UsuarioId.Equals(idUsuario));
          
                usuario.UsuarioConfig.UsarEmail = request.UsarEmail;
                usuario.UsuarioConfig.UsarNumeroCelular = request.UsarNumeroCelular;
                dbContext.SaveChanges();
                
                return Results.Ok();
            }).WithTags("Autenticação");
        }
    }
}
