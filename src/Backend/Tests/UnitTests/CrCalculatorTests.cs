using ControleDisciplinas.Domain.Services;
using Xunit;

namespace ControleDisciplinas.Tests.UnitTests;

/// <summary>Casos exigidos pelo roteiro §16 para o cálculo do CR (decisão D4).</summary>
public class CrCalculatorTests
{
    [Fact]
    public void HistoricoVazio_RetornaNulo()
    {
        Assert.Null(CrCalculator.Calcular([]));
    }

    [Fact]
    public void DisciplinaSemCreditos_NaoEntraNoCalculo()
    {
        // só a disciplina de 0 créditos: sem créditos somados -> CR nulo
        Assert.Null(CrCalculator.Calcular([(9.0m, 0)]));

        // 0 créditos junto de disciplinas normais: ignorada
        var cr = CrCalculator.Calcular([(10.0m, 0), (5.0m, 4)]);
        Assert.Equal(5.00m, cr);
    }

    [Fact]
    public void UmaDisciplina_CrIgualAMedia()
    {
        var cr = CrCalculator.Calcular([(7.5m, 4)]);
        Assert.Equal(7.50m, cr);
    }

    [Fact]
    public void MultiplasDisciplinas_MediaPonderadaPorCreditos()
    {
        // (8*4 + 6*2 + 10*4) / 10 = (32 + 12 + 40) / 10 = 8.4
        var cr = CrCalculator.Calcular([(8m, 4), (6m, 2), (10m, 4)]);
        Assert.Equal(8.40m, cr);
    }

    [Fact]
    public void NotasComCasasDecimais_CalculoExato()
    {
        // (7.25*4 + 8.75*2) / 6 = (29 + 17.5) / 6 = 7.75
        var cr = CrCalculator.Calcular([(7.25m, 4), (8.75m, 2)]);
        Assert.Equal(7.75m, cr);
    }

    [Fact]
    public void Arredondamento_DuasCasasMeioParaCima()
    {
        // (7*3 + 8*3) / 6 = 7.5 -> exato; caso com dízima:
        // (7*1 + 8*1 + 8*1) / 3 = 7.666... -> 7.67
        var cr = CrCalculator.Calcular([(7m, 1), (8m, 1), (8m, 1)]);
        Assert.Equal(7.67m, cr);

        // meio exato: (7.125*2)/2 = 7.125 -> 7.13 (AwayFromZero)
        var cr2 = CrCalculator.Calcular([(7.125m, 2)]);
        Assert.Equal(7.13m, cr2);
    }

    [Fact]
    public void ConjuntosDiferentes_NaoSeMisturam()
    {
        // garante que o cálculo é puro por coleção (isolamento entre usuários
        // é coberto fim a fim em IntegrationTests/IsolamentoTests)
        var alunoA = CrCalculator.Calcular([(10m, 4)]);
        var alunoB = CrCalculator.Calcular([(2m, 4)]);
        Assert.Equal(10.00m, alunoA);
        Assert.Equal(2.00m, alunoB);
    }
}
