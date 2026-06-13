using ControleDisciplinas.Api.Extensions;
using ControleDisciplinas.Api.Middlewares;
using ControleDisciplinas.Application;
using ControleDisciplinas.Application.Interfaces;
using ControleDisciplinas.Infrastructure;
using ControleDisciplinas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddRestrictiveCors(builder.Configuration);
builder.Services.AddSwaggerWithJwt();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("sqlite");

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

// Swagger somente em desenvolvimento (segurança 18)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(ServiceCollectionExtensions.CorsPolicy);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check sem expor informações sensíveis (segurança 17)
app.MapHealthChecks("/health");

// Inicialização: migrations + importação de CSV (somente no início da aplicação — normativo §2).
// O ambiente "Testing" pula esta etapa; os testes controlam o banco e a importação diretamente.
if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var dataSource = db.Database.GetDbConnection().DataSource;
    if (!string.IsNullOrEmpty(dataSource))
    {
        var dir = Path.GetDirectoryName(Path.GetFullPath(dataSource));
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
    }

    db.Database.Migrate();

    var importador = scope.ServiceProvider.GetRequiredService<ICsvImportService>();
    var resultado = await importador.ImportarAsync();
    if (resultado.Executada)
        app.Logger.LogInformation("CSV importado: {Arquivo} (+{Importados}, ={Ignorados}, !{Invalidas})",
            resultado.Arquivo, resultado.Importados, resultado.Ignorados, resultado.LinhasInvalidas);

    await DemoSeed.ExecutarAsync(scope.ServiceProvider, app.Configuration, app.Environment, app.Logger);
}

app.Run();

public partial class Program; // exposto para os testes de integração (WebApplicationFactory)
