using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Request;
using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Response;
using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.OTP;
using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.OTPSender;
using API.BOBERTO.AUTHENTICATION.DOMAIN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.BOBERTO.AUTHENTICATION.WEB.Routes
{
    public static class OtpRoute
    {
        public static void AddOtpRoute(this WebApplication app)
        {
            app.MapPost("/otp/enviarCodigoSMS",
                [Authorize(AuthenticationSchemes = "api_key")] (
                [FromBody] SendOTPSMSRequest request,
               [FromServices] DatabaseContext dbContext,
                IOTPService otpCode,
                IOTPSender enviarCodigoDuploFator
            ) =>
              {
                  request.Validar();
                  var codigo = otpCode.Generate();
                  enviarCodigoDuploFator.SendSMS(request.NumeroCelular, codigo);
                  return Results.Ok();

              }).WithTags("Dupla autenticação");

            app.MapPost("/otp/enviarCodigoEmail", [Authorize(AuthenticationSchemes = "api_key")] (
            [FromBody] EnviarCodigoEmailRequest request,
            [FromServices] DatabaseContext dbContext,
            IOTPService otpCode,
            IOTPSender enviarCodigoDuploFator) =>
            {
                request.Validate();
                var codigo = otpCode.Generate();
                enviarCodigoDuploFator.SendEmail(request.Email, codigo);
                return Results.Ok();

            }).WithTags("Dupla autenticação");

            app.MapPost("/otp/gerarotp", [Authorize(AuthenticationSchemes = "api_key")] (
            [FromServices] DatabaseContext dbContext,
            IOTPService otpCode) =>
            {
                var codigo = otpCode.Generate();
                return Results.Ok(new GenerateOtpResponse()
                {
                    Codigo = codigo
                });

            }).WithTags("Dupla autenticação");

            app.MapPost("/otp/validarotp", [Authorize(AuthenticationSchemes = "api_key")] (
              [FromBody] TwoFactorVerifyRequest request,
            IOTPService otpCode
              ) =>
          {
              request.Validate();
              var valid = otpCode.Validate(request.Code);

              return Results.Ok(valid);

          }).WithTags("Dupla autenticação");
        }
    }
}
