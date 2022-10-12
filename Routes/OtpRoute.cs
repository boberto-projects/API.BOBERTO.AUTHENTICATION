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
          app.MapPost("/otp/enviarCodigoSMS", [Authorize] async ([FromServices] DatabaseContext dbContext,
          IOTPCode otpCode,
          IEnviarCodigoDuploFator enviarCodigoDuploFator,
          IUsuarioService usuarioLogado) =>
            {
                var usuarioAtual = usuarioLogado.ObterUsuarioLogado();

                var codigo = otpCode.GerarCodigoOTP();

                enviarCodigoDuploFator.EnviarCodigoSMS(usuarioLogado, codigo);

                return Results.Ok();

            }).WithTags("Dupla autenticação");

            app.MapPost("/otp/enviarCodigoEmail", [Authorize] async ([FromServices] DatabaseContext dbContext,
            IOTPCode otpCode,
            IEnviarCodigoDuploFator enviarCodigoDuploFator,
            IUsuarioService usuarioLogado) =>
            {
                var usuarioAtual = usuarioLogado.ObterUsuarioLogado();

                var codigo = otpCode.GerarCodigoOTP();

                enviarCodigoDuploFator.EnviarCodigoEmail(usuarioLogado, codigo);

                return Results.Ok(new GenerateOtpResponse()
                {
                    Code = codigo
                });

            }).WithTags("Dupla autenticação");

            app.MapPost("/otp/gerarotp", [AllowAnonymous]  async ([FromServices] DatabaseContext dbContext,
            IOTPCode otpCode) =>
            {
                var codigo = otpCode.GerarCodigoOTP();

                return Results.Ok(new GenerateOtpResponse()
                {
                    Code = codigo
                });

            }).WithTags("Dupla autenticação");

            app.MapPost("/otp/validarotp", [AllowAnonymous] ([FromBody] TwoFactorVerifyRequest request,
               IRedisService redisService,
              IOTPCode otpCode
                ) =>
            {
                var valid = otpCode.ValidarCodigoOTP(request.Code);

                if (valid.Valido)
                {
                    redisService.Clear(request.ObterChaveCache);
                }

                return Results.Ok(valid);

            }).WithTags("Dupla autenticação");
        }
    }
}
