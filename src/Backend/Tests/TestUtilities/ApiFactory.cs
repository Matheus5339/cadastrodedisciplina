using System.Net.Http.Headers;
using System.Net.Http.Json;
using ControleDisciplinas.Application.DTOs;
using ControleDisciplinas.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ControleDisciplinas.Tests.TestUtilities;

/// <summary>
/// Sobe a API real em memória (ambiente "Testing") com SQLite in-memory
/// compartilhado e configuração de teste injetada (segredo JWT de teste,
/// Argon2 com custo reduzido para velocidade).
/// </summary>
public sealed class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    static ApiFactory()
    {
        // Com minimal hosting o Program lê a configuração no topo do arquivo,
        // antes dos callbacks da factory — por isso os valores de teste vão
        // por variável de ambiente (lidas na criação do WebApplicationBuilder).
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
                .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>)
                         || d.ServiceType == typeof(DbContextOptions))
                .ToList();
            foreach (var d in descritores)
                services.Remove(d);

            services.AddDbContext<AppDbContext>(o => o.UseSqlite(_connection));
        });
    }

    async Task IAsyncLifetime.InitializeAsync()
    {
        await _connection.OpenAsync();
        using var scope = Services.CreateScope();
        await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreatedAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await base.DisposeAsync();
        await _connection.DisposeAsync();
    }

    private static int _seq;

    /// <summary>Registra um aluno novo (dados únicos) e devolve client autenticado + tokens.</summary>
    public async Task<(HttpClient Client, AuthResultDto Auth)> RegistrarAlunoAsync(string? nome = null)
    {
        var n = Interlocked.Increment(ref _seq);
        var client = CreateClient();
        var request = new
        {
            rgu = $"20261{n:D5}",
            cpf = GerarCpfValido(n),
            email = $"aluno{n}@teste.ucp.edu.br",
            nome = nome ?? $"Aluno Teste {n}",
            senha = "SenhaForte123",
        };

        var resposta = await client.PostAsJsonAsync("/api/auth/register", request);
        resposta.EnsureSuccessStatusCode();
        var auth = (await resposta.Content.ReadFromJsonAsync<AuthResultDto>())!;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
        return (client, auth);
    }

    /// <summary>Gera um CPF com dígitos verificadores válidos a partir de uma semente.</summary>
    public static string GerarCpfValido(int semente)
    {
        var numeros = new int[11];
        var baseStr = (100000000 + semente).ToString("D9");
        for (var i = 0; i < 9; i++)
            numeros[i] = baseStr[i] - '0';

        for (var dv = 9; dv < 11; dv++)
        {
            var soma = 0;
            for (var i = 0; i < dv; i++)
                soma += numeros[i] * (dv + 1 - i);
            numeros[dv] = soma * 10 % 11 % 10;
        }

        return string.Concat(numeros);
    }
}
