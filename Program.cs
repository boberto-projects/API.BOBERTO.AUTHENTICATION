using api_authentication_boberto;
using api_authentication_boberto.Models;
using ConfigurationSubstitution;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NuGet.Configuration;
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
    return dbContext.Usuarios.ToList();
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
        Senha = hashed
    });
    await dbContext.SaveChangesAsync();
})
.WithTags("Autenticação");

app.MapGet("/teste", [Authorize] async ( [FromServices] DatabaseContext dbContext) =>
{
    return Results.Ok();
})
.WithTags("Autenticação");


app.Run();


