using api_authentication_boberto;
using api_authentication_boberto.CustomDbContext;
using api_authentication_boberto.Integrations;
using api_authentication_boberto.Integrations.Zenvia;
using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Models.Integrations;
using api_authentication_boberto.Routes;
using api_authentication_boberto.Services.Implements;
using ConfigurationSubstitution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RestEase;
using System.Security.Claims;
using static api_authentication_boberto.Integrations.Zenvia.Request.SendSMSRequest;

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

app.MapGet("/", ([FromServices] ApiCicloDeVida apiCicloDeVida) =>
{
    var ultimoDeploy = "�ltimo deploy " + apiCicloDeVida.iniciouEm.ToString("dd/MM/yyyy HH:mm:ss");
    var upTime = DateTime.Now.Subtract(apiCicloDeVida.iniciouEm);
    var ambiente = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

    return ultimoDeploy + Environment.NewLine + "Ambiente:" + ambiente + Environment.NewLine + upTime;
}).WithTags("Health Check");

app.MapPost("/teste", ([FromServices] GerenciadorZenvio gerenciadorZenvio,
    [FromServices] IOptions<DiscordAPIConfig> discordApiConfig,
    [FromServices] IZenviaApi zEnviaAPI,
    [FromServices] IOptions<ZenviaApiConfig> zeenviaApiConfig
    ) =>
{
    //testando SMS e limite de envio de SMS di�rio.

    var chave = "COUNT_SMS_GLOBAL_SENDED";
    //gerenciando envio de sms para limite di�rio
    gerenciadorZenvio.IncrementarTentativa(chave);

    if (gerenciadorZenvio.AtingiuLimiteMaximoDeTentativas(chave))
    {
        return Results.Ok("Limite m�ximo de SMS di�rio atingido.");
    }

    var mensagem = new List<Content>();
    mensagem.Add(new()
    {
        Type = "text",
        Text = "ApiBobertoAuth: Seu codigo e"
    });

    zEnviaAPI.EnviarSMS(new()
    {
        To = "numeroCelularTeste",
        From = zeenviaApiConfig.Value.Alias,
        Contents = mensagem
    }).Wait();

    return Results.Ok();
}).WithTags("Health Check");



app.Run();


