using api_authentication_boberto.Domain.CustomDbContext;
using api_authentication_boberto.Exceptions;
using api_authentication_boberto.Interfaces;
using api_authentication_boberto.Models;
using api_authentication_boberto.Models.Response;
using api_authentication_boberto.Services.Interfaces;
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

            app.MapPost("/ativarDuplaAutenticacao", [Authorize] (
            [FromBody] AtivarDuplaAutenticacaoRequest request,
            [FromServices] DatabaseContext dbContext,
            IOTPCode otpCode,
            IUsuarioService usuarioLogado) =>
            {
                request.Validar();

                var codigoOtpValido = otpCode.ValidarCodigoOTP(request.Codigo).Valido;

                if (codigoOtpValido == false)
                {
                    throw new CustomException(StatusCodeEnum.Negocio, "Código inválido.");
                }

                var emailValido = string.IsNullOrEmpty(request.Email) == false;
                var numeroCelularValido = string.IsNullOrEmpty(request.NumeroCelular) == false;

                usuarioLogado.AtivarAutenticacaoDupla(
                new AutenticacaoDupla()
                {
                    Email = request.Email,
                    UsarEmail = emailValido,
                    NumeroCelular = request.NumeroCelular,
                    UsarNumeroCelular = numeroCelularValido
                });

                return Results.Ok();
            }).WithTags("Usuário");
        }
    }
}
