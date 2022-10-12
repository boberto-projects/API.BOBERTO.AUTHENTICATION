using api_authentication_boberto;
using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Routes;
using api_authentication_boberto.Services.Implements;
using ConfigurationSubstitution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
if (config.GetSection("ApiConfig").Get<ApiConfig>().Swagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.AdicionarLoginRoute();
app.AdicionarOtpRoute();
app.AdicionarUsuarioRoute();
app.AdicionarApiConfigRoute();

app.MapGet("/", ([FromServices] ApiCicloDeVida apiCicloDeVida) =>
{
    var ultimoDeploy = "Último deploy " + apiCicloDeVida.iniciouEm.ToString("dd/MM/yyyy HH:mm:ss");
    var upTime = DateTime.Now.Subtract(apiCicloDeVida.iniciouEm);
    var ambiente = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

    return ultimoDeploy + Environment.NewLine + "Ambiente:" + ambiente + Environment.NewLine + upTime;
}).WithTags("Health Check");


app.MapPost("/teste", [Authorize(AuthenticationSchemes = "ApiKeyAuthenticationHandler")] ([FromServices] GerenciadorZenvio gerenciadorZenvio,
    [FromServices] IOptions<DiscordAPIConfig> discordApiConfig,
    [FromServices] IOptions<ZenviaApiConfig> zeenviaApiConfig
    ) =>
{


   

    return Results.Ok();
}).WithTags("Health Check");



app.Run();


