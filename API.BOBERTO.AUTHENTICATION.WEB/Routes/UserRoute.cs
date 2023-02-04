using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions;
using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions.Models;
using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Models;
using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Response;
using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.CurrentUser;
using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.OTP;
using API.BOBERTO.AUTHENTICATION.DOMAIN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.BOBERTO.AUTHENTICATION.WEB.Routes
{
    public static class UserRoute
    {
        public static void AddUserRoute(this WebApplication app)
        {
            app.MapGet("/perfil", [Authorize] (
                ICurrentUserService currentUser) =>
            {
                return currentUser.GetCurrentProfile();
            }).WithTags("Usuário");

            app.MapPost("/ativarDuplaAutenticacao", [Authorize] (
            IOTPService otpService,
            ICurrentUserService currentUserService,
            [FromBody] EnablePairAuthenticationRequest request,
            [FromServices] DatabaseContext dbContext) =>
            {
                request.Validate();

                var codigoOtpValido = otpService.Validate(request.Code).Valid;

                if (codigoOtpValido == false)
                {
                    throw new CustomException(StatusCodeEnum.BUSINESS, "Código inválido.");
                }

                var emailValido = string.IsNullOrEmpty(request.Email) == false;
                var numeroCelularValido = string.IsNullOrEmpty(request.PhoneNumber) == false;

                currentUserService.EnablePairAuthentication(
                new PairAuthentication()
                {
                    Email = request.Email,
                    UsarEmail = emailValido,
                    NumeroCelular = request.PhoneNumber,
                    UsarNumeroCelular = numeroCelularValido
                });

                return Results.Ok();
            }).WithTags("Usuário");
        }
    }
}
