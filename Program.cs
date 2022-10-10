using api_authentication_boberto;
using api_authentication_boberto.Implements;
using api_authentication_boberto.Interfaces;
using api_authentication_boberto.Models.Config;
using api_authentication_boberto.Routes;
using ConfigurationSubstitution;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MinecraftServer.Api.Services;
using System.Security.Claims;
using System.Text;

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
builder.Services.AddStackExchangeRedisCache(options =>
{
options.Configuration = config.GetConnectionString("Redis");
});

builder.Services.Configure<DiscordAPIConfig>(options => config.GetSection("DiscordAPIConfig").Bind(options));
builder.Services.Configure<TwoFactorConfig>(options => config.GetSection("TwoFactorConfig").Bind(options));

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IRedisService, RedisService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

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


