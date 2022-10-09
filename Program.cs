using api_authentication_boberto;
using ConfigurationSubstitution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddEntityFrameworkNpgsql().AddDbContext<DatabaseContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("Postgree")));


var app = builder.Build();

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

app.MapPost("/autenticar", () =>
{
  //  return config.GetSection("ConnectionStrings").GetSection("PostGree");
})
.WithTags("Autenticação");

app.MapPost("/registrar", async ([FromBody] RegistrarRequest request, [FromServices] DatabaseContext dbContext) =>
{
    dbContext.Usuarios.Add(new()
    {
        Email = request.Email,
        Nome =  request.Nome,
        Senha = request.Senha
    });
    await dbContext.SaveChangesAsync();
})
.WithTags("Autenticação");

app.Run();

class LoginRequest
{
    public string Email { get; set; }
    public string Senha { get; set; }
}

class RegistrarRequest
{
    public string Email { get; set; }
    public string Senha { get; set; }
    public string Nome { get; set; }
}
