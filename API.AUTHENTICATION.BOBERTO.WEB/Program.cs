using API.AUTHENTICATION.BOBERTO.WEB.Bootstrap;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using API.BOBERTO.AUTHENTICATION.APPLICATION.MESSAGES.Config;
using API.BOBERTO.AUTHENTICATION.WEB.Routes;
using API.BOBERTO.AUTHENTICATION.WEB.Handlers;
using API.BOBERTO.AUTHENTICATION.WEB;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

/// We need to put this at BobertoNuggetLibrary
///TODO: we need to delegate this to other pattern.
builder.AddConfigurations();
builder.AddServices();
builder.AddPostgreeStorage();
builder.AddRedisStorage();
builder.AddIntegrations();
builder.AddAuthentications();

var app = builder.Build();

app.AddCustomExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Services.GetRequiredService<IOptions<ApiConfig>>().Value.Swagger)
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

app.AddLoginRoute();
app.AddApiKeyRoute();
app.AddOtpRoute();
app.AddUserRoute();
app.AddResourceRoute();

///We need to put this at BobertoNuggetLibrary
app.MapGet("/", ([FromServices] HealthCheck healthCheck) =>
{
    var upTime = healthCheck.GetUpTime();
    return healthCheck;
}).WithTags("Health Check");

app.MapGet("/teste", [AuthorizeAttribute(AuthenticationSchemes = "user_api_key")] () =>
{
    return Results.Ok("OK");
}).WithTags("Health Check")
.RequireAuthorization("modpack_manage");

app.Run();