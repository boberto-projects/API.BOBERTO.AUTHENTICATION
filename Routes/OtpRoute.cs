using api_authentication_boberto.CustomDbContext;
using api_authentication_boberto.Interfaces;
using api_authentication_boberto.Models;
using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Models.Request;
using api_authentication_boberto.Models.Response;
using api_authentication_boberto.Services.Implements;
using api_authentication_boberto.Services.Interfaces;
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
            app.MapPost("/gerarotp", [Authorize]  async ([FromServices] DatabaseContext dbContext,
            IOTPCode otpCode,
            IEnviarCodigoDuploFator enviarCodigoDuploFator,
            IUsuarioService usuarioLogado) =>
            {
                var usuarioAtual = usuarioLogado.ObterUsuarioLogado();

                var codigo = otpCode.GerarCodigoOTP();

                enviarCodigoDuploFator.EnviarCodigo(usuarioLogado, codigo);

                return Results.Ok(new GenerateOtpResponse()
                {
                    Code = codigo
                });

            }).WithTags("Autenticação");

            //USANDO Time-based OTPs
            app.MapPost("/validarotp", [AllowAnonymous] ([FromBody] TwoFactorVerifyRequest request,
              IOTPCode otpCode
                ) =>
            {
                var valid = otpCode.ValidarCodigoOTP(request.Code);

                return Results.Ok(valid);

            }).WithTags("Autenticação");

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
            }).WithTags("Autenticação");
        }
    }
}
