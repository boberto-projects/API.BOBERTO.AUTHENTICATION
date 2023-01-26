using api_authentication_boberto;
using api_authentication_boberto.DependencyInjection;
using api_authentication_boberto.Exceptions;
using api_authentication_boberto.Interfaces;
using api_authentication_boberto.Models;
using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Routes;
using api_authentication_boberto.Services.Implements;
using ConfigurationSubstitution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;

var builder = WebApplication.CreateBuilder(args);

/// We need to put this at BobertoNuggetLibrary
var config = new ConfigurationBuilder()
.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
.AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
.AddEnvironmentVariables()
.EnableSubstitutions("%", "%")
.Build();

///TODO: we need to delegate this to other pattern.
builder.AddConfigurations();
builder.AddPostgreeStorage();
builder.AddRedisStorage();
builder.AddAuthentications();
builder.AddServices();
builder.AddIntegrations();

var app = builder.Build();

///We need to put this at BobertoNuggetLibrary
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";
        var exceptionHandlerPathFeature =
            context.Features.Get<IExceptionHandlerPathFeature>();

        if (exceptionHandlerPathFeature?.Error is CustomException customException)
        {
            context.Response.StatusCode = customException.CodigoDeStatus;
            await context.Response.WriteAsJsonAsync(customException.ObterResponse());
        }
        if (exceptionHandlerPathFeature?.Error is CodigoOTPException codigoOTPException)
        {
            context.Response.StatusCode = codigoOTPException.CodigoDeStatus;
            await context.Response.WriteAsJsonAsync(codigoOTPException.ObterResponse());
        }
    });
});

// Configure the HTTP request pipeline.
if (config.GetSection("ApiConfig").Get<ApiConfig>().Swagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();


app.AdicionarLoginRoute();
app.AddApiKeyRoute();
app.AdicionarOtpRoute();
app.AdicionarUsuarioRoute();
app.AdicionarApiConfigRoute();

///We need to put this at BobertoNuggetLibrary
app.MapGet("/", ([FromServices] ApiCicloDeVida apiCicloDeVida) =>
{
    var ultimoDeploy = "Last deploy " + apiCicloDeVida.iniciouEm.ToString("dd/MM/yyyy HH:mm:ss");
    var upTime = DateTime.Now.Subtract(apiCicloDeVida.iniciouEm);
    var ambiente = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

    return ultimoDeploy + Environment.NewLine + "Ambiente:" + ambiente + Environment.NewLine + upTime;
}).WithTags("Health Check");


app.MapGet("/teste", [AuthorizeAttribute(AuthenticationSchemes = "user_api_key")] () =>
{
    return Results.Ok();
}).WithTags("Health Check")
.RequireAuthorization("modpack_manage");




app.Run();


