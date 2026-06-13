using ControleDisciplinas.Infrastructure.Csv;
using ControleDisciplinas.Infrastructure.Persistence;
using ControleDisciplinas.Infrastructure.Persistence.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace ControleDisciplinas.Tests.IntegrationTests;

/// <summary>Casos exigidos pelo roteiro §15 para a importação de CSV.</summary>
public sealed class CsvImportServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _db;
    private readonly string _dir;

    public CsvImportServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        _db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options);
        _db.Database.EnsureCreated();
        _dir = Directory.CreateTempSubdirectory("csv-import-tests-").FullName;
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
        try { Directory.Delete(_dir, recursive: true); } catch { /* best effort */ }
    }

    private CsvImportService CriarServico(CsvImportOptions? opt = null)
    {
        opt ??= new CsvImportOptions { Directory = _dir };
        return new CsvImportService(
            new DisciplinaRepository(_db),
            new ImportLogRepository(_db),
            _db,
            Options.Create(opt),
            NullLogger<CsvImportService>.Instance);
    }

    private string EscreverCsv(string nome, string conteudo)
    {
        var caminho = Path.Combine(_dir, nome);
        File.WriteAllText(caminho, conteudo);
        return caminho;
    }

    private const string Cabecalho = "codigo,nome,professor,periodo,creditos";

    [Fact]
    public async Task ArquivoAusente_NaoExecutaENaoFalha()
    {
        var resultado = await CriarServico().ImportarAsync();

        Assert.False(resultado.Executada);
        Assert.Equal(0, await _db.Disciplinas.CountAsync());
        Assert.Equal(0, await _db.ImportLogs.CountAsync());
    }

    [Fact]
    public async Task ArquivoValido_ImportaTodasAsLinhas()
    {
        EscreverCsv("disciplinas.csv", $"{Cabecalho}\nECOMP-001,Calculo I,Fulano,1,4\nECOMP-002,Fisica I,,1,4\n");

        var resultado = await CriarServico().ImportarAsync();

        Assert.True(resultado.Executada);
        Assert.Equal(2, resultado.Importados);
        Assert.Equal(0, resultado.LinhasInvalidas);
        Assert.Equal(2, await _db.Disciplinas.CountAsync());
        Assert.Equal(1, await _db.ImportLogs.CountAsync());
        var semProfessor = await _db.Disciplinas.SingleAsync(d => d.Codigo == "ECOMP-002");
        Assert.Null(semProfessor.Professor); // professor vazio vira null
    }

    [Fact]
    public async Task LinhaInvalida_EhContadaEIgnorada_SemImpedirAsDemais()
    {
        EscreverCsv("disciplinas.csv",
            $"{Cabecalho}\nECOMP-001,Calculo I,Fulano,1,4\nSEM-CAMPOS-SUFICIENTES,X\n,SemCodigo,Y,1,4\nECOMP-003,Creditos invalidos,Z,1,abc\nECOMP-004,Valida,W,2,2\n");

        var resultado = await CriarServico().ImportarAsync();

        Assert.Equal(2, resultado.Importados);
        Assert.Equal(3, resultado.LinhasInvalidas);
        Assert.Equal(2, await _db.Disciplinas.CountAsync());
    }

    [Fact]
    public async Task Duplicidade_NoArquivoENoBanco_NaoDuplicaRegistros()
    {
        EscreverCsv("disciplinas.csv", $"{Cabecalho}\nECOMP-001,Calculo I,Fulano,1,4\nECOMP-001,Calculo I repetida,Outro,1,4\n");
        var primeira = await CriarServico().ImportarAsync();
        Assert.Equal(1, primeira.Importados);
        Assert.Equal(1, primeira.Ignorados); // duplicada dentro do próprio arquivo

        EscreverCsv("disciplinas.csv", $"{Cabecalho}\nECOMP-001,Calculo I,Fulano,1,4\nECOMP-002,Nova,Prof,1,2\n");
        var segunda = await CriarServico().ImportarAsync();

        Assert.Equal(1, segunda.Importados); // só a nova
        Assert.Equal(1, segunda.Ignorados);  // a que já estava no banco
        Assert.Equal(2, await _db.Disciplinas.CountAsync());
    }

    [Fact]
    public async Task ExecucaoRepetida_EhIdempotente()
    {
        EscreverCsv("disciplinas.csv", $"{Cabecalho}\nECOMP-001,Calculo I,Fulano,1,4\nECOMP-002,Fisica I,Prof,1,4\n");

        var primeira = await CriarServico().ImportarAsync();
        var segunda = await CriarServico().ImportarAsync();

        Assert.Equal(2, primeira.Importados);
        Assert.Equal(0, segunda.Importados);
        Assert.Equal(2, segunda.Ignorados);
        Assert.Equal(2, await _db.Disciplinas.CountAsync());
        Assert.Equal(2, await _db.ImportLogs.CountAsync()); // cada execução é auditada
    }

    [Fact]
    public async Task Separador_VirgulaComCamposEntreAspas_OutroSeparadorEhInvalido()
    {
        EscreverCsv("disciplinas.csv",
            $"{Cabecalho}\nECOMP-022,\"Fluidos, Ondas e Calor\",Prof,3,4\nECOMP-001;PontoEVirgula;Prof;1;4\n");

        var resultado = await CriarServico().ImportarAsync();

        Assert.Equal(1, resultado.Importados);
        Assert.Equal(1, resultado.LinhasInvalidas); // linha com ';' não tem 5 campos por vírgula
        var d = await _db.Disciplinas.SingleAsync();
        Assert.Equal("Fluidos, Ondas e Calor", d.Nome);
    }

    [Fact]
    public async Task CaminhoConfiguravel_PathExplicitoTemPrioridade_EAliasEhAceito()
    {
        // alias do normativo (com typo) encontrado pela lista de FileNames
        EscreverCsv("discipolinas.csv", $"{Cabecalho}\nECOMP-001,Via alias,Prof,1,4\n");
        var porAlias = await CriarServico().ImportarAsync();
        Assert.True(porAlias.Executada);
        Assert.EndsWith("discipolinas.csv", porAlias.Arquivo);

        // Path explícito ignora o diretório/aliases
        var explicito = EscreverCsv("qualquer-nome.csv", $"{Cabecalho}\nECOMP-099,Via path,Prof,9,4\n");
        var porPath = await CriarServico(new CsvImportOptions { Path = explicito, Directory = _dir }).ImportarAsync();

        Assert.True(porPath.Executada);
        Assert.Equal(explicito, porPath.Arquivo);
        Assert.Single(await _db.Disciplinas.Where(d => d.Codigo == "ECOMP-099").ToListAsync());
    }
}
