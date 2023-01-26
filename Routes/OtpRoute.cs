﻿using api_authentication_boberto.Domain.CustomDbContext;
using api_authentication_boberto.Interfaces;
using api_authentication_boberto.Models;
using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Models.Request;
using api_authentication_boberto.Models.Response;
using api_authentication_boberto.Services.Implements;
using api_authentication_boberto.Services.OTPSender;
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
          app.MapPost("/otp/enviarCodigoSMS", [Authorize(AuthenticationSchemes = "ApiKeyAuthenticationHandler")] (
              [FromBody] EnviarCodigoSMSRequest request,
             [FromServices] DatabaseContext dbContext,
              IOTPService otpCode,
              IOTPSender enviarCodigoDuploFator
          ) =>
            {
                request.Validar();
                var codigo = otpCode.GerarCodigoOTP();
                enviarCodigoDuploFator.EnviarCodigoSMS(request.NumeroCelular, codigo);

                ///TODO: essas tentativas de sincronizar timezone tá bem chatinha :c
                ///Próximo plano é configurar um DateTime global para toda a api. Possivelmente usando timezone America/Sao_Paulo

                return Results.Ok();

            }).WithTags("Dupla autenticação");

            app.MapPost("/otp/enviarCodigoEmail", [Authorize(AuthenticationSchemes = "ApiKeyAuthenticationHandler")] (
            [FromBody] EnviarCodigoEmailRequest request,
            [FromServices] DatabaseContext dbContext,
            IOTPService otpCode,
            IOTPSender enviarCodigoDuploFator) =>
            {
                request.Validar();
                var codigo = otpCode.GerarCodigoOTP();
                enviarCodigoDuploFator.EnviarCodigoEmail(request.Email, codigo);
                return Results.Ok();

            }).WithTags("Dupla autenticação");

            app.MapPost("/otp/gerarotp", [Authorize(AuthenticationSchemes = "ApiKeyAuthenticationHandler")] (
            [FromServices] DatabaseContext dbContext,
            IOTPService otpCode) =>
            {
                var codigo = otpCode.GerarCodigoOTP();
                return Results.Ok(new GenerateOtpResponse()
                {
                    Codigo = codigo
                });

            }).WithTags("Dupla autenticação");

              app.MapPost("/otp/validarotp", [Authorize(AuthenticationSchemes = "ApiKeyAuthenticationHandler")] (
                [FromBody] TwoFactorVerifyRequest request,
              IOTPService otpCode
                ) =>
            {
                request.Validar();
                var valid = otpCode.ValidarCodigoOTP(request.Codigo);
              
                return Results.Ok(valid);

            }).WithTags("Dupla autenticação");
        }
    }
}
