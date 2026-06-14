using System.Threading.RateLimiting;
using ControleDisciplinas.Api;
using ControleDisciplinas.Api.Extensions;
using ControleDisciplinas.Api.Middlewares;
using ControleDisciplinas.Application;
using ControleDisciplinas.Infrastructure;
using ControleDisciplinas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Logging estruturado (P1 — observabilidade): legível em desenvolvimento,
// JSON (pronto para agregadores) em produção.
builder.Logging.ClearProviders();
if (builder.Environment.IsDevelopment())
    builder.Logging.AddSimpleConsole(o => { o.SingleLine = true; o.TimestampFormat = "HH:mm:ss "; });
else
    builder.Logging.AddJsonConsole(o => { o.IncludeScopes = true; o.UseUtcTimestamp = true; });

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(
        new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddRestrictiveCors(builder.Configuration);
builder.Services.AddSwaggerWithJwt();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("sqlite");

// Rate limiting (P0): protege os endpoints de autenticação contra força bruta.
// Desativado no ambiente "Testing" para não interferir na suíte de integração.
var habilitarRateLimit = !builder.Environment.IsEnvironment("Testing");
if (habilitarRateLimit)
{
    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        options.AddPolicy(RateLimitPolicies.Auth, http =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: http.Connection.RemoteIpAddress?.ToString() ?? "desconhecido",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0,
                }));
    });
}

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

// HSTS + redirecionamento HTTPS apenas fora de Development/Testing
// (o ambiente de desenvolvimento roda em HTTP).
if (!app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Testing"))
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

// Swagger somente em desenvolvimento (segurança 18)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(ServiceCollectionExtensions.CorsPolicy);

if (habilitarRateLimit)
    app.UseRateLimiter();

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

    await Seed.ExecutarAsync(scope.ServiceProvider, app.Logger);
}

app.Run();

public partial class Program; // exposto para os testes de integração (WebApplicationFactory)
