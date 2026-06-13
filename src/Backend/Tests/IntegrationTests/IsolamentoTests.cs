using System.Net;
using System.Net.Http.Json;
using ControleDisciplinas.Application.DTOs;
using ControleDisciplinas.Tests.TestUtilities;
using Xunit;

namespace ControleDisciplinas.Tests.IntegrationTests;

/// <summary>Isolamento por aluno (segurança 11–13) validado fim a fim.</summary>
public class IsolamentoTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private static async Task<DisciplinaDto> CriarDisciplinaAsync(HttpClient client, string codigo, int creditos = 4)
    {
        var resposta = await client.PostAsJsonAsync("/api/disciplinas",
            new { codigo, nome = $"Disciplina {codigo}", professor = "Prof Teste", periodo = 1, creditos });
        resposta.EnsureSuccessStatusCode();
        return (await resposta.Content.ReadFromJsonAsync<DisciplinaDto>())!;
    }

    private static async Task<HistoricoDto> LancarHistoricoAsync(HttpClient client, int disciplinaId, decimal media, int ano = 2025)
    {
        var resposta = await client.PostAsJsonAsync("/api/historicos",
            new { disciplinaId, ano, semestre = 1, periodo = 1, mediaFinal = media });
        resposta.EnsureSuccessStatusCode();
        return (await resposta.Content.ReadFromJsonAsync<HistoricoDto>())!;
    }

    [Fact]
    public async Task HistoricoECr_DeUmAluno_NaoVazamParaOutro()
    {
        var (alunoA, _) = await factory.RegistrarAlunoAsync("Aluno A");
        var (alunoB, _) = await factory.RegistrarAlunoAsync("Aluno B");

        var disciplina = await CriarDisciplinaAsync(alunoA, $"ISO-{Guid.NewGuid().ToString("N")[..6]}");
        var lancamentoA = await LancarHistoricoAsync(alunoA, disciplina.Id, 9.0m);

        // B não vê o histórico de A
        var historicoB = await alunoB.GetFromJsonAsync<List<HistoricoDto>>("/api/historicos");
        Assert.DoesNotContain(historicoB!, h => h.Id == lancamentoA.Id);

        // B não consegue alterar nem remover o lançamento de A (404, sem vazar existência)
        var put = await alunoB.PutAsJsonAsync($"/api/historicos/{lancamentoA.Id}",
            new { disciplinaId = disciplina.Id, ano = 2025, semestre = 1, periodo = 1, mediaFinal = 1.0m });
        Assert.Equal(HttpStatusCode.NotFound, put.StatusCode);

        var delete = await alunoB.DeleteAsync($"/api/historicos/{lancamentoA.Id}");
        Assert.Equal(HttpStatusCode.NotFound, delete.StatusCode);

        // CR de cada um é calculado só com os próprios lançamentos
        var crA = await alunoA.GetFromJsonAsync<CrDto>("/api/historicos/cr");
        var crB = await alunoB.GetFromJsonAsync<CrDto>("/api/historicos/cr");
        Assert.Equal(9.00m, crA!.Cr);
        Assert.Null(crB!.Cr); // B não cursou nada

        // o lançamento de A permanece intacto
        var historicoA = await alunoA.GetFromJsonAsync<List<HistoricoDto>>("/api/historicos");
        Assert.Contains(historicoA!, h => h.Id == lancamentoA.Id && h.MediaFinal == 9.0m);
    }

    [Fact]
    public async Task IdAlunoNuncaVemDoCorpo_SempreDoToken()
    {
        var (alunoA, authA) = await factory.RegistrarAlunoAsync();
        var (alunoB, authB) = await factory.RegistrarAlunoAsync();

        var disciplina = await CriarDisciplinaAsync(alunoA, $"TOK-{Guid.NewGuid().ToString("N")[..6]}");

        // mesmo que B envie "alunoId" de A no corpo, o lançamento é gravado para B (campo é ignorado)
        var resposta = await alunoB.PostAsJsonAsync("/api/historicos",
            new { alunoId = authA.Aluno.Id, disciplinaId = disciplina.Id, ano = 2024, semestre = 2, periodo = 1, mediaFinal = 5.0m });
        resposta.EnsureSuccessStatusCode();

        var historicoA = await alunoA.GetFromJsonAsync<List<HistoricoDto>>("/api/historicos");
        var historicoB = await alunoB.GetFromJsonAsync<List<HistoricoDto>>("/api/historicos");
        Assert.DoesNotContain(historicoA!, h => h.Ano == 2024 && h.Semestre == 2 && h.DisciplinaId == disciplina.Id);
        Assert.Contains(historicoB!, h => h.Ano == 2024 && h.Semestre == 2 && h.DisciplinaId == disciplina.Id);
        Assert.NotEqual(authA.Aluno.Id, authB.Aluno.Id);
    }

    [Fact]
    public async Task PerfilEFoto_SaoDoAlunoDoToken()
    {
        var (alunoA, authA) = await factory.RegistrarAlunoAsync("Aluno Perfil A");
        var (alunoB, _) = await factory.RegistrarAlunoAsync("Aluno Perfil B");

        var meA = await alunoA.GetFromJsonAsync<AlunoDto>("/api/alunos/me");
        var meB = await alunoB.GetFromJsonAsync<AlunoDto>("/api/alunos/me");
        Assert.Equal(authA.Aluno.Email, meA!.Email);
        Assert.NotEqual(meA.Id, meB!.Id);
    }
}
