using System.Net;
using System.Net.Http.Json;
using ControleDisciplinas.Application.DTOs;
using ControleDisciplinas.Tests.TestUtilities;
using Xunit;

namespace ControleDisciplinas.Tests.IntegrationTests;

public class DisciplinasEHistoricoTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task CrudDeDisciplina_ComCodigoUnico()
    {
        var (client, _) = await factory.RegistrarAlunoAsync();
        var codigo = $"CRUD-{Guid.NewGuid().ToString("N")[..6]}";

        // criar
        var criada = await (await client.PostAsJsonAsync("/api/disciplinas",
            new { codigo, nome = "Estruturas de Dados", professor = "Prof X", periodo = 3, creditos = 4 }))
            .Content.ReadFromJsonAsync<DisciplinaDto>();
        Assert.NotNull(criada);

        // código duplicado -> 409
        var duplicada = await client.PostAsJsonAsync("/api/disciplinas",
            new { codigo, nome = "Outra", professor = "Y", periodo = 1, creditos = 2 });
        Assert.Equal(HttpStatusCode.Conflict, duplicada.StatusCode);

        // atualizar
        var atualizada = await (await client.PutAsJsonAsync($"/api/disciplinas/{criada!.Id}",
            new { codigo, nome = "Estruturas de Dados II", professor = "Prof Z", periodo = 4, creditos = 4 }))
            .Content.ReadFromJsonAsync<DisciplinaDto>();
        Assert.Equal("Estruturas de Dados II", atualizada!.Nome);

        // remover
        var remocao = await client.DeleteAsync($"/api/disciplinas/{criada.Id}");
        Assert.Equal(HttpStatusCode.NoContent, remocao.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/disciplinas/{criada.Id}")).StatusCode);
    }

    [Fact]
    public async Task Filtros_PorNomeProfessorAnoESemestre()
    {
        var (client, _) = await factory.RegistrarAlunoAsync();
        var sufixo = Guid.NewGuid().ToString("N")[..6];

        var d1 = await (await client.PostAsJsonAsync("/api/disciplinas",
            new { codigo = $"FA-{sufixo}", nome = $"Algoritmos Especiais {sufixo}", professor = "Maria Silva", periodo = 2, creditos = 4 }))
            .Content.ReadFromJsonAsync<DisciplinaDto>();
        var d2 = await (await client.PostAsJsonAsync("/api/disciplinas",
            new { codigo = $"FB-{sufixo}", nome = $"Banco de Dados Especial {sufixo}", professor = "Joao Souza", periodo = 4, creditos = 4 }))
            .Content.ReadFromJsonAsync<DisciplinaDto>();

        // filtro por nome
        var porNome = await client.GetFromJsonAsync<List<DisciplinaDto>>($"/api/disciplinas?nome=Algoritmos Especiais {sufixo}");
        Assert.Single(porNome!);
        Assert.Equal(d1!.Id, porNome![0].Id);

        // filtro por professor
        var porProfessor = await client.GetFromJsonAsync<List<DisciplinaDto>>("/api/disciplinas?professor=Joao Souza");
        Assert.Contains(porProfessor!, d => d.Id == d2!.Id);
        Assert.DoesNotContain(porProfessor!, d => d.Id == d1.Id);

        // ano/semestre cruzam com o histórico do aluno autenticado (decisão D10)
        await client.PostAsJsonAsync("/api/historicos", new { disciplinaId = d1.Id, ano = 2023, semestre = 1, periodo = 2, mediaFinal = 8.0m });
        await client.PostAsJsonAsync("/api/historicos", new { disciplinaId = d2!.Id, ano = 2024, semestre = 2, periodo = 4, mediaFinal = 7.0m });

        var por2023 = await client.GetFromJsonAsync<List<DisciplinaDto>>("/api/disciplinas?ano=2023");
        Assert.Contains(por2023!, d => d.Id == d1.Id);
        Assert.DoesNotContain(por2023!, d => d.Id == d2.Id);

        var por2024S2 = await client.GetFromJsonAsync<List<DisciplinaDto>>("/api/disciplinas?ano=2024&semestre=2");
        Assert.Contains(por2024S2!, d => d.Id == d2.Id);
        Assert.DoesNotContain(por2024S2!, d => d.Id == d1.Id);

        // filtros do histórico
        var hist2023 = await client.GetFromJsonAsync<List<HistoricoDto>>("/api/historicos?ano=2023");
        Assert.Single(hist2023!);
        var histProf = await client.GetFromJsonAsync<List<HistoricoDto>>("/api/historicos?professor=Maria");
        Assert.Single(histProf!);
    }

    [Fact]
    public async Task Historico_LancamentoDuplicado_Retorna409_EDisciplinaComHistoricoNaoPodeSerRemovida()
    {
        var (client, _) = await factory.RegistrarAlunoAsync();
        var disciplina = await (await client.PostAsJsonAsync("/api/disciplinas",
            new { codigo = $"HD-{Guid.NewGuid().ToString("N")[..6]}", nome = "Compiladores", professor = "P", periodo = 7, creditos = 4 }))
            .Content.ReadFromJsonAsync<DisciplinaDto>();

        var corpo = new { disciplinaId = disciplina!.Id, ano = 2025, semestre = 1, periodo = 7, mediaFinal = 6.5m };
        (await client.PostAsJsonAsync("/api/historicos", corpo)).EnsureSuccessStatusCode();

        var duplicado = await client.PostAsJsonAsync("/api/historicos", corpo);
        Assert.Equal(HttpStatusCode.Conflict, duplicado.StatusCode);

        var remocao = await client.DeleteAsync($"/api/disciplinas/{disciplina.Id}");
        Assert.Equal(HttpStatusCode.Conflict, remocao.StatusCode);
    }

    [Fact]
    public async Task Cr_CalculadoComMediaPonderada_ViaApi()
    {
        var (client, _) = await factory.RegistrarAlunoAsync();
        var sufixo = Guid.NewGuid().ToString("N")[..6];

        var d4 = await (await client.PostAsJsonAsync("/api/disciplinas",
            new { codigo = $"CR4-{sufixo}", nome = "Quatro Creditos", professor = "P", periodo = 1, creditos = 4 }))
            .Content.ReadFromJsonAsync<DisciplinaDto>();
        var d2 = await (await client.PostAsJsonAsync("/api/disciplinas",
            new { codigo = $"CR2-{sufixo}", nome = "Dois Creditos", professor = "P", periodo = 1, creditos = 2 }))
            .Content.ReadFromJsonAsync<DisciplinaDto>();

        await client.PostAsJsonAsync("/api/historicos", new { disciplinaId = d4!.Id, ano = 2025, semestre = 1, periodo = 1, mediaFinal = 8.0m });
        await client.PostAsJsonAsync("/api/historicos", new { disciplinaId = d2!.Id, ano = 2025, semestre = 1, periodo = 1, mediaFinal = 5.0m });

        var cr = await client.GetFromJsonAsync<CrDto>("/api/historicos/cr");

        // (8*4 + 5*2) / 6 = 42/6 = 7.00
        Assert.Equal(7.00m, cr!.Cr);
        Assert.Equal(6, cr.TotalCreditos);
        Assert.Equal(2, cr.TotalDisciplinas);
    }

    [Fact]
    public async Task Validacao_MediaForaDoIntervalo_Retorna400()
    {
        var (client, _) = await factory.RegistrarAlunoAsync();
        var disciplina = await (await client.PostAsJsonAsync("/api/disciplinas",
            new { codigo = $"VAL-{Guid.NewGuid().ToString("N")[..6]}", nome = "Valida", professor = "P", periodo = 1, creditos = 4 }))
            .Content.ReadFromJsonAsync<DisciplinaDto>();

        var resposta = await client.PostAsJsonAsync("/api/historicos",
            new { disciplinaId = disciplina!.Id, ano = 2025, semestre = 1, periodo = 1, mediaFinal = 11.0m });
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }
}
