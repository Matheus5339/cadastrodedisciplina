using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using ControleDisciplinas.Api;
using ControleDisciplinas.Application.DTOs;
using ControleDisciplinas.Application.Interfaces;
using ControleDisciplinas.Domain.Entities;
using ControleDisciplinas.Domain.Enums;
using ControleDisciplinas.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ControleDisciplinas.Tests.TestUtilities;

/// <summary>
/// Sobe a API real em memória (ambiente "Testing") com SQLite in-memory compartilhado.
/// Faz o seed de um usuário de cada perfil e do álbum único.
/// </summary>
public sealed class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public const string Senha = "Teste@123";

    /// <summary>Opções JSON dos testes — aceitam enums como string (igual à API).</summary>
    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    static ApiFactory()
    {
        Environment.SetEnvironmentVariable("Jwt__Secret", new string('s', 64) + "-apenas-para-testes");
        Environment.SetEnvironmentVariable("PasswordHash__MemoryCostKib", "8192");
        Environment.SetEnvironmentVariable("PasswordHash__TimeCost", "1");
        Environment.SetEnvironmentVariable("PasswordHash__Lanes", "1");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            var descritores = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>) || d.ServiceType == typeof(DbContextOptions))
                .ToList();
            foreach (var d in descritores) services.Remove(d);
            services.AddDbContext<AppDbContext>(o => o.UseSqlite(_connection));
        });
    }

    async Task IAsyncLifetime.InitializeAsync()
    {
        await _connection.OpenAsync();
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();

        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        db.Usuarios.AddRange(
            new Usuario("admin", hasher.Hash(Senha), Perfil.Administrador),
            new Usuario("autor", hasher.Hash(Senha), Perfil.Autor),
            new Usuario("colecionador", hasher.Hash(Senha), Perfil.Colecionador));
        db.Albuns.Add(new Album("Álbum de Teste", 10));
        await db.SaveChangesAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await base.DisposeAsync();
        await _connection.DisposeAsync();
    }

    /// <summary>Faz login e devolve um client com o Bearer já configurado.</summary>
    public async Task<HttpClient> ClienteAsync(string login)
    {
        var client = CreateClient();
        var resp = await client.PostAsJsonAsync("/api/auth/login", new { login, senha = Senha });
        resp.EnsureSuccessStatusCode();
        var auth = (await resp.Content.ReadFromJsonAsync<AuthResponseDto>(Json))!;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
        return client;
    }

    public Task<HttpClient> ClienteAdminAsync() => ClienteAsync("admin");
    public Task<HttpClient> ClienteAutorAsync() => ClienteAsync("autor");
    public Task<HttpClient> ClienteColecionadorAsync() => ClienteAsync("colecionador");

    /// <summary>Imagem PNG mínima distinta por semente (tags MD5 diferentes).</summary>
    public static byte[] ImagemFake(int semente)
    {
        var png = Convert.FromBase64String(
            "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==");
        return [.. png, .. BitConverter.GetBytes(semente)];
    }

    public static MultipartFormDataContent FormFigurinha(int numero, string nome, int pagina, string? descricao, int sementeImagem)
    {
        var form = new MultipartFormDataContent
        {
            { new StringContent(numero.ToString()), "Numero" },
            { new StringContent(nome), "Nome" },
            { new StringContent(pagina.ToString()), "Pagina" },
        };
        if (descricao is not null) form.Add(new StringContent(descricao), "Descricao");
        var img = new ByteArrayContent(ImagemFake(sementeImagem));
        img.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        form.Add(img, "Imagem", $"fig{sementeImagem}.png");
        return form;
    }
}
