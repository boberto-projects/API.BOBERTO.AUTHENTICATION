using api_authentication_boberto;
using api_authentication_boberto.Models;
using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Models.Request;
using ConfigurationSubstitution;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OtpNet;
using System.Text;
using BC = BCrypt.Net.BCrypt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .EnableSubstitutions("%", "%")
            .Build();
            var jwtKey = Encoding.ASCII.GetBytes(config["Jwt:Key"]);
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
               .AddJwtBearer(x =>
               {
                   x.RequireHttpsMetadata = false;
                   x.SaveToken = true;
                   x.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
                       ValidateIssuer = false,
                       ValidateAudience = false
                   };
               });

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddEntityFrameworkNpgsql().AddDbContext<DatabaseContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("Postgree")));


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/healthcheck", ([FromServices] DatabaseContext dbContext) =>
{
    return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
})
.WithTags("Autenticação");

app.MapPost("/autenticar", [AllowAnonymous] ([FromBody] LoginRequest request, [FromServices] DatabaseContext dbContext, [FromServices] IConfiguration config) =>
{
    var contaCadastrada = dbContext.Usuarios.FirstOrDefault(e => e.Email.Equals(request.Email));

    var contaExiste = contaCadastrada != null;

    if(contaExiste == false)
    {
        return Results.Unauthorized();
    }

    var senhaCorreta = BC.Verify(request.Senha, contaCadastrada.Senha);

    if(senhaCorreta == false)
    {
       return Results.Unauthorized();
    }

    return Results.Ok(new LoginResult(){
        Token = contaCadastrada.GerarTokenJWT(config)
    });

})
.WithTags("Autenticação");

app.MapPost("/registrar", [AllowAnonymous] async ([FromBody] RegistrarRequest request, [FromServices] DatabaseContext dbContext) =>
{
    string hashed = BC.HashPassword(request.Senha);

    dbContext.Usuarios.Add(new()
    {
        Email = request.Email,
        Nome =  request.Nome,
        Senha = hashed,
        NumeroCelular = request.NumeroCelular

    });
    await dbContext.SaveChangesAsync();
})
.WithTags("Autenticação");

app.MapGet("/teste", [Authorize] async ([FromServices] DatabaseContext dbContext) =>
{
    return Results.Ok();
})
.WithTags("Autenticação");


app.MapGet("/generateOtp", [AllowAnonymous] async ([FromServices] IConfiguration config) =>
{
    var twoFactorConfig = config.GetSection("TwoFactorConfig").Get<TwoFactorConfig>();

    var key = Encoding.ASCII.GetBytes(twoFactorConfig.Key);
    var size = twoFactorConfig.Size;

    var totp = new Totp(key, totpSize: size);

    var code = totp.ComputeTotp();

    return Results.Ok(code);

});

app.MapPost("/validOtp", [AllowAnonymous] async ([FromBody] TwoFactorVerifyRequest request, [FromServices] IConfiguration config) =>
{
    var twoFactorConfig = config.GetSection("TwoFactorConfig").Get<TwoFactorConfig>();

    var key = Encoding.ASCII.GetBytes(twoFactorConfig.Key);
    var size = twoFactorConfig.Size;

    var totp = new Totp(key, totpSize: size);

    var valid = totp.VerifyTotp(request.Code, out long timeStepMatched);

    return Results.Ok(valid);

})
.WithTags("Autenticação");


app.Run();


