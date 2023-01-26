using api_authentication_boberto.DependencyInjection;
using api_authentication_boberto.Exceptions;
using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Routes;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

/// We need to put this at BobertoNuggetLibrary


///TODO: we need to delegate this to other pattern.
builder.AddConfigurations();
builder.AddServices();
builder.AddPostgreeStorage();
builder.AddRedisStorage();
builder.AddAuthentications();
builder.AddIntegrations();

var app = builder.Build();

///We need to put this at BobertoNuggetLibrary

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


app.AdicionarLoginRoute();
app.AddApiKeyRoute();
app.AdicionarOtpRoute();
app.AdicionarUsuarioRoute();
app.AdicionarApiConfigRoute();

///We need to put this at BobertoNuggetLibrary
//app.MapGet("/", ([FromServices] ApiCicloDeVida apiCicloDeVida) =>
//{
//    var ultimoDeploy = "Last deploy " + apiCicloDeVida.StartAt.ToString("dd/MM/yyyy HH:mm:ss");
//    var upTime = apiCicloDeVida.GetUpTime();
//    var environment = apiCicloDeVida.Environment;
//    return apiCicloDeVida;
//}).WithTags("Health Check");


//app.MapGet("/teste", [AuthorizeAttribute(AuthenticationSchemes = "user_api_key")] () =>
//{
//    return Results.Ok();
//}).WithTags("Health Check")
//.RequireAuthorization("modpack_manage");

app.Run();


