﻿using api_authentication_boberto.Domain.CustomDbContext;
using api_authentication_boberto.Exceptions;
using api_authentication_boberto.Models;
using api_authentication_boberto.Models.Enums;
using api_authentication_boberto.Models.Response;
using api_authentication_boberto.Services.CurrentUser;
using api_authentication_boberto.Services.OTP;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_authentication_boberto.Routes
{
    public static class UserRoute
    {
        public static void AddUserRoute(this WebApplication app)
        {
            app.MapGet("/perfil", [Authorize] ([FromServices] ICurrentUserService usuarioLogado) =>
            {
                return usuarioLogado.ObterUsuarioLogado();
            }).WithTags("Usuário");

            app.MapPost("/ativarDuplaAutenticacao", [Authorize] (
            [FromBody] AtivarDuplaAutenticacaoRequest request,
            [FromServices] DatabaseContext dbContext,
            IOTPService otpCode,
            ICurrentUserService usuarioLogado) =>
            {
                request.Validar();

                var codigoOtpValido = otpCode.Validate(request.Codigo).Valido;

                if (codigoOtpValido == false)
                {
                    throw new CustomException(StatusCodeEnum.BUSINESS, "Código inválido.");
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