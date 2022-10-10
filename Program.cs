using api_authentication_boberto;
using api_authentication_boberto.CustomDbContext;
using api_authentication_boberto.Routes;
using ConfigurationSubstitution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);


var config = new ConfigurationBuilder()
.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
.AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
.AddEnvironmentVariables()
.EnableSubstitutions("%", "%")
.Build();


builder.InjetarConfiguracoes(config);
builder.InjetarServicosDeArmazenamento(config);
builder.InjetarServicosAutenticacao(config);
builder.InjetarServicos(config);
builder.InjetarIntegracoes(config);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
app.UseSwagger();
app.UseSwaggerUI();
}

app.AdicionarLoginRoute();
app.AdicionarOtpRoute();

app.MapGet("/healthcheck", ([FromServices] DatabaseContext dbContext) =>
{
return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
})
.WithTags("Autenticação");

app.MapGet("/teste", [Authorize] async ([FromServices] DatabaseContext dbContext, ClaimsPrincipal user) =>
{
var usuarioId = user.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
.Select(c => c.Value).SingleOrDefault();
return Results.Ok(usuarioId);
   
})
.WithTags("Autenticação");

app.Run();


