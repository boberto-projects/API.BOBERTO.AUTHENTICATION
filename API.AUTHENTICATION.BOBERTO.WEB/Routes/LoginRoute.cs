using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Enums.Authentication;
using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Enums.OTP;
using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions;
using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Exceptions.Models;
using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Request;
using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Response;
using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.CurrentUser;
using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.JWT;
using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.OTP;
using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.OTPSender;
using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.Redis;
using API.BOBERTO.AUTHENTICATION.APPLICATION.Services.UserSecurity;
using API.BOBERTO.AUTHENTICATION.DOMAIN;
using API.BOBERTO.AUTHENTICATION.DOMAIN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace API.BOBERTO.AUTHENTICATION.WEB.Routes
{
    public static class LoginRoute
    {
        public static void AddLoginRoute(this WebApplication app)
        {
            app.MapPost("/autenticar", [Authorize(AuthenticationSchemes = "api_key")] ([FromBody] LoginRequest request,
                [FromServices] DatabaseContext dbContext,
                IOTPService otpCode,
                IRedisService redisService,
                IOTPSender enviarCodigoDuploFator,
                IUserSecurity userSecurity,
                IConfiguration config,
                IJWTService jwtService
                ) =>
            {
                request.Validate();

                var accountRegistred = dbContext.Usuarios.Include(c => c.UserConfig).FirstOrDefault(e => e.Email.Equals(request.Email));

                if (accountRegistred == null)
                {
                    throw new CustomException(StatusCodeEnum.NOTAUTHORIZED, "Not authorized");
                }
                userSecurity.CreateUserCache(accountRegistred);
                var maxLimitAttempts = userSecurity.ReachedMaximumLimitOfAttempts();
                ///Pra um usuário com dupla autenticação ativa, ele sempre terá UsarNumeroCelular true e UsarEmail true.
                ///Então eu posso verificar se esse usuário informou um código de OTP pra saber se ele pode logar ou não.
                var pairAuthenticationEnabled = accountRegistred.UserConfig.EnabledPhoneNumber || accountRegistred.UserConfig.EnabledEmail;

                var codeOTPExists = string.IsNullOrEmpty(request.Code) == false;

                var codeOTP = codeOTPExists && otpCode.Validate(request.Code).Valid;

                ///Se atingiu o limite máximo de tentativas de login falhas e o codigo otp não foi informado
                if (maxLimitAttempts && codeOTPExists == false)
                {

                    throw new CustomException(StatusCodeEnum.NOTAUTHORIZED, "You've mistyped the password too many times. Wait a while before trying again.");
                }

                ///Comparo a senha.
                var senhaCorreta = BC.Verify(request.Password, accountRegistred?.Password);

                if (senhaCorreta == false)
                {
                    userSecurity.IncrementAttemp();
                    throw new CustomException(StatusCodeEnum.NOTAUTHORIZED, "Invalid data.");
                }

                ///Se a dupla autenticação estiver ativa e o código não for informado ou inválido.. Eu vou retornar um token do tipo token_temporario
                ///um token de 35 segundos que o aplicativo usuará apenas para obter informações
                if (pairAuthenticationEnabled)
                {
                    throw new CodeOTPException(OTPEnum.OTPInvalid, "Wrong OTP code");
                }
                ///Código informado mas não é válido
                if (codeOTPExists && codeOTP == false)
                {
                    throw new CodeOTPException(OTPEnum.OTPInvalid, "Wrong OTP code");
                }
                /// Atualizo que o UltimoLogin do usuário e retorno um sucesso com o JWT.
                accountRegistred.LastLogin = DateTime.Now;
                var token = jwtService.Generate(accountRegistred);
                dbContext.SaveChanges();

                return Results.Ok(new LoginResponse()
                {
                    PairAuthenticationEnabled = pairAuthenticationEnabled,
                    Token = token
                });
            }).WithTags("Autenticação");

            ///Wrong way to do this. The best form is foreach authentication api informs a random key to application using AES encrypt schema.
            ///The application needs to send this encrypt key when found a 401 route.
            //refresh token
            app.MapPost("/refresh_token", [Authorize] (
                [FromServices] DatabaseContext dbContext,
                [FromBody] RefreshTokenRequest request,
                ICurrentUserService usuarioLogado,
                IJWTService tokenJWTService) =>
            {
                var tokenValido = tokenJWTService.Validate(request.Token);
                if (tokenValido == false)
                {
                    throw new CustomException(StatusCodeEnum.NOTAUTHORIZED, "Token inválido");
                }
                var usuario = usuarioLogado.GetCurrentProfile();
                var expiracao = DateTime.UtcNow.AddHours(1);
                var token = tokenJWTService.Generate(new UserModel()
                {
                    UserId = usuario.Id,
                    Email = usuario.Email,
                    Name = usuario.Nome,
                    PhoneNumber = usuario.NumeroCelular
                }, expiracao);

                return Results.Ok(new RefreshTokenResponse()
                {
                    Token = token,
                    ExpiraEm = expiracao
                });
            });

            app.MapPost("/registrar", [Authorize(AuthenticationSchemes = "api_key")] (
                [FromBody] RegisterRequest request,
                [FromServices] DatabaseContext dbContext) =>
        {
            request.Valid();

            var emailEmUso = dbContext.Usuarios.Count(x => x.Email.Equals(request.Email)) > 0;

            if (emailEmUso)
            {
                throw new CustomException(StatusCodeEnum.BUSINESS, "Account already exists");
            }

            string hashed = BC.HashPassword(request.Password);

            var usuarioConfig = new UserConfigModel();

            dbContext.UsuariosConfig.Add(usuarioConfig);
            dbContext.SaveChanges();

            dbContext.Usuarios.Add(new()
            {
                Email = request.Email,
                Name = request.Name,
                Password = hashed,
                PhoneNumber = request.PhoneNumber,
                UserConfigId = usuarioConfig.UserConfigId,
                Role = RolesEnum.USER
            });

            dbContext.SaveChanges();

        }).WithTags("Usuário");
        }
    }
}
